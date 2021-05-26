using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SemperPrecisStageTracker.API.Middlewares;
using SemperPrecisStageTracker.Domain.Configurations;
using ZenProgramming.Chakra.Core.Configurations;
using ZenProgramming.Chakra.Core.Diagnostic;

namespace SemperPrecisStageTracker.API
{
    public class Startup
    {
        readonly string localPolicy = "CorsPolicy";
        readonly string productionPolicy = "ProductionPolicy";

        public IConfiguration Configuration { get; }

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
            Configuration = configuration;

            //Definizione del nome e versione del sistema
            ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
            ApplicationVersion = $"v{Assembly.GetEntryAssembly().GetName().Version.Major}" +
                                 $".{Assembly.GetEntryAssembly().GetName().Version.Minor}" +
                                 $".{Assembly.GetEntryAssembly().GetName().Version.Build}";
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy(name: localPolicy,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                options.AddPolicy(name: productionPolicy,
                    builder => builder
                        .WithOrigins("https://semperprecisStagetracker.azurewebsites.net")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });


            //Aggiungo l'autentications basic e il default di schema
            services
                .AddAuthentication(o => o.DefaultScheme = BasicAuthenticationOptions.Scheme)
                .AddBasicAuthentication();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SemperPrecisStageTracker", Version = "v1" });

                //c.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
                //{
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "basic",
                //    Description = "Input your username and password to access this API",
                //    In = ParameterLocation.Header,
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "basicAuth"
                //            }
                //        },
                //        new List<string>()
                //    }
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SemperPrecisStageTracker v1"));
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            //Abilito CORS
            //Se siamo in modalit� "dev"
            switch (ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.EnvironmentName.ToLower())
            {
                case "production":
                    Tracer.Info($"[CORS] Working on {productionPolicy} CORS policy");
                    app.UseCors(productionPolicy);
                    break;
                case "development":
                    Tracer.Info($"[CORS] Working on {localPolicy} CORS policy");
                    app.UseCors(localPolicy);
                    break;
                default:
                    throw new Exception("CORS configuration NOT FOUND");
            }
            
            ////Utilizzo l'autenticazione
            //app.UseAuthentication();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hi!");
                });
                endpoints.MapControllers();
            });
        }
    }
}
