using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.OpenApi.Models;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Middlewares;
using SemperPrecisStageTracker.API.Models;
using SemperPrecisStageTracker.API.Services;
using SemperPrecisStageTracker.API.Services.Interfaces;
using SemperPrecisStageTracker.Domain.Cache;
using SemperPrecisStageTracker.Domain.Clients;
using SemperPrecisStageTracker.Domain.Configurations;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Services;
using SemperPrecisStageTracker.EF.Clients;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Mocks.Clients;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models.Diagnostics.Tracers;
using ZenProgramming.Chakra.Core.Configurations;
using ZenProgramming.Chakra.Core.Configurations.Utils;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Diagnostic;
using ZenProgramming.Chakra.Core.EntityFramework.Data;
using ZenProgramming.Chakra.Core.Mocks.Data;

namespace SemperPrecisStageTracker.API;

public class Program
{
    static readonly string corsPolicy = nameof(corsPolicy);

    /// <summary>
    /// Application name
    /// </summary>
    public static string ApplicationName { get; private set; }

    /// <summary>
    /// Application version
    /// </summary>
    public static string ApplicationVersion { get; private set; }

    static void Main(string[] args)
    {
        //Configurazione del tracer
        Tracer.Append(typeof(Log4NetTracer));
        Tracer.Info($"[Settings] Working on environment '{ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.EnvironmentName}' (from configuration factory)");

        //Select provider for data storage
        SettingsUtils.Switch(ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider, new Dictionary<string, Action>
        {
            { "Mockup", SessionFactory.RegisterDefaultDataSession<MockDataSession<SimpleScenario>> },
            { "Sql", SessionFactory.RegisterDefaultDataSession<EntityFrameworkDataSession<SemperPrecisStageTrackerContext>> }
        });
        // identity client
        SettingsUtils.Switch(ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider, new Dictionary<string, Action>
        {
            { "Mockup", ServiceResolver.Register<IIdentityClient, MockIdentityClient> },
            { "Sql", ServiceResolver.Register<IIdentityClient, SqlIdentityClient> }
        });
        ServiceResolver.Register<ISemperPrecisMemoryCache, SemperPrecisMemoryCache>();

        ServiceResolver.Register<ICaptchaValidatorService, CaptchaValidatorService>();


        //Definizione del nome e versione del sistema
        var assembly = Assembly.GetEntryAssembly()?.GetName() ?? throw new NullReferenceException("Assembly not found");

        ApplicationName = assembly.Name;
        ApplicationVersion = $"v{assembly.Version?.Major ?? 0}" +
                             $".{assembly.Version?.Minor ?? 0}" +
                             $".{assembly.Version?.Build ?? 0}";

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureAppConfiguration((hostContext, config) =>
        {
            var isDev = hostContext.HostingEnvironment.IsDevelopment();
            if (isDev)
            {
                config.AddUserSecrets<Program>();
            }

            var settings = config.Build();

            if (!isDev)
            {
                // use az key vault
                var kvName = settings["azKVName"];

                if (string.IsNullOrEmpty(kvName))
                    throw new NullReferenceException("Azure KeyVault name not provided");

                var vaultName = $"https://{kvName}.vault.azure.net/";

                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
                config.AddAzureKeyVault(vaultName, keyVaultClient, new DefaultKeyVaultSecretManager());
            }
        });
        // register configuration across application
        ServiceResolver.Register<IConfiguration>(builder.Configuration);
        

        builder.Services.AddCors(options =>
        {
            var blazorEndpoints = builder.Configuration.GetSection("blazorEndpoints").Get<string[]>();
            if (blazorEndpoints != null && blazorEndpoints.Length > 0)
            {
                options.AddPolicy(name: corsPolicy,
                    builder => builder
                        .WithOrigins(blazorEndpoints)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            }
            else
            {
                options.AddPolicy(name: corsPolicy, (builder) => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            }
        });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
        }
        else
        {
            // smtp
            var emailConfig = builder.Configuration
                                .GetSection("EmailConfiguration")
                                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();
        }
        


        //Aggiungo l'autentications basic e il default di schema
        builder.Services
            .AddAuthentication(o => o.DefaultScheme = BasicAuthenticationOptions.Scheme)
            .AddBasicAuthentication();


        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.WriteIndented = false;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });


        builder.Services.AddHealthChecks();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SemperPrecisStageTracker", Version = "v1" });

            c.OperationFilter<PermissionsFilter>();

            c.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "Input your username and password to access this API",
                In = ParameterLocation.Header,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basicAuth"
                        }
                    },
                    new List<string>()
                }
            });
        });

        var app = builder.Build();
        var enableDev = bool.Parse(builder.Configuration["enableDev"] ?? "false");
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SemperPrecisStageTracker v1"));
        }
        else
        {
            if (enableDev)
            {
                app.UseDeveloperExceptionPage();
            }
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        //Abilito CORS
        app.UseCors(corsPolicy);

        //Utilizzo l'autenticazione
        app.UseAuthentication();

        app.UseAuthorization();

        if (app.Environment.IsDevelopment() || enableDev)
        {
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new Dictionary<string, IList<string>>
                            {
                                {"",new List<string>{exception.Message} }
                            };

                await context.Response.WriteAsJsonAsync(response);
            }));
        }
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/healthz");
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hi!");
            });
            endpoints.MapControllers();
        });
        app.Run();
    }
}