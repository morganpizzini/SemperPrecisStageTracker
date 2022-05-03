using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SemperPrecisStageTracker.API.Helpers;
using SemperPrecisStageTracker.API.Middlewares;
using SemperPrecisStageTracker.Domain.Configurations;
using SemperPrecisStageTracker.Domain.Containers;
using ZenProgramming.Chakra.Core.Configurations;
using ZenProgramming.Chakra.Core.Diagnostic;

namespace SemperPrecisStageTracker.API
{
    public class Startup
    {
        readonly string corsPolicy = nameof(corsPolicy);
        /// <summary>
        /// Application name
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Application version
        /// </summary>
        public string ApplicationVersion { get; }

        public Startup(IConfiguration configuration)
        {
            ServiceResolver.Register(configuration);

            //Definizione del nome e versione del sistema
            ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
            ApplicationVersion = $"v{Assembly.GetEntryAssembly().GetName().Version.Major}" +
                                 $".{Assembly.GetEntryAssembly().GetName().Version.Minor}" +
                                 $".{Assembly.GetEntryAssembly().GetName().Version.Build}";
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = ServiceResolver.Resolve<IConfiguration>();
            services.AddCors(options =>
            {
                var blazorEndpoints = configuration.GetSection("blazorEndpoints").Get<string[]>();
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


            //Aggiungo l'autentications basic e il default di schema
            services
                .AddAuthentication(o => o.DefaultScheme = BasicAuthenticationOptions.Scheme)
                .AddBasicAuthentication();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddHealthChecks();

            services.AddSwaggerGen(c =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SemperPrecisStageTracker v1"));
            }
            else
            {
                if (bool.Parse(configuration["enableDev"] ?? "false"))
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hi!");
                });
                endpoints.MapControllers();
            });
        }
    }
}
