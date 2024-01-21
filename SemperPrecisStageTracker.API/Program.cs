using System.Reflection;
using System.Text.Json;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
using SemperPrecisStageTracker.Domain.Services.Mocks;
using SemperPrecisStageTracker.EF.Clients;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Mocks.Clients;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models.Diagnostics.Tracers;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZenProgramming.Chakra.Core.Configurations;
using ZenProgramming.Chakra.Core.Configurations.Utils;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Diagnostic;
using ZenProgramming.Chakra.Core.EntityFramework.Data;
using ZenProgramming.Chakra.Core.Mocks.Data;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Mvc;

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
        var isDev = builder.Environment.IsDevelopment();
        if (isDev)
        {
            builder.Services.AddScoped<IKeyVaultService, KeyVaultServiceMock>();
        }
        else
        {
            var keyvaultConnection = builder.Configuration.GetConnectionString("KeyVault");

            if (string.IsNullOrEmpty(keyvaultConnection))
                throw new ArgumentNullException($"Connection string {nameof(keyvaultConnection)} is empty");

            var keyvaultUri = new Uri(keyvaultConnection);
            var client = new SecretClient(keyvaultUri, new DefaultAzureCredential());
            builder.Configuration.AddAzureKeyVault(
                keyvaultUri,
                new DefaultAzureCredential(),
                new AzureKeyVaultConfigurationOptions()
                {
                    Manager = new SampleKeyVaultSecretManager(),
                    ReloadInterval = TimeSpan.FromMinutes(10)
                });

            builder.Services.AddSingleton<SecretClient>(client);
            builder.Services.AddScoped<IKeyVaultService, KeyVaultService>();

        }

        // register configuration across application
        ServiceResolver.Register<IConfiguration>(builder.Configuration);

        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressInferBindingSourcesForParameters = true);
        //builder.Services.AddProblemDetails();
        builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        builder.Services
            .AddApiVersioning(options =>
            {
                //indicating whether a default version is assumed when a client does
                // does not provide an API version.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
            })
            .AddApiExplorer(options =>
            {
                // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

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

        builder.Services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                            new HeaderApiVersionReader("x-api-version"),
                                                            new MediaTypeApiVersionReader("x-api-version"));
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();
            //c.SwaggerDoc("v1", new OpenApiInfo { Title = "SemperPrecisStageTracker", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

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
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        var app = builder.Build();

        var enableDev = bool.Parse(builder.Configuration["enableDev"] ?? "false");
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                var descriptions = app.DescribeApiVersions();

                // Build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });
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

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Example Web API",
            Version = description.ApiVersion.ToString(),
            Description = "Description for the example Web API",
            Contact = new OpenApiContact { Name = "Author name", Email = "author-main@org.com" },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                 description.DefaultValue != null &&
                 description.DefaultValue is not DBNull &&
                 description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}