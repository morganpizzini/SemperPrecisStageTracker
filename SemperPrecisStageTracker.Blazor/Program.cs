using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.Localization;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace SemperPrecisStageTracker.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                    options.ValidationMessageLocalizer = (message, arguments) =>
                    {
                        var stringLocalizer = options.Services.GetService<IStringLocalizer<Program>>();

                        return stringLocalizer != null && arguments?.Count() > 0
                            ? string.Format(stringLocalizer[message], arguments.ToArray())
                            : message;
                    };
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            builder.Services
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<NetworkService>();
            
            //builder.Services.AddScoped(x =>
            //{
            //    Console.WriteLine("TestHttpClient");
            //    var apiUrl = new Uri(builder.Configuration["baseAddress"]);
            //    // use fake backend if "fakeBackend" is "true" in appsettings.json
            //    // if (builder.Configuration["fakeBackend"] == "true")
            //    //     return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

            //    return new HttpClient() { BaseAddress = apiUrl };
            //});

            // load configuration from server

            var apiUrl = new Uri(builder.Configuration["baseAddress"]);

            // use fake backend if "fakeBackend" is "true" in appsettings.json
            // if (builder.Configuration["fakeBackend"] == "true")
            //     return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

            var httpClient = new HttpClient() { BaseAddress = apiUrl };

            //configure http client
            builder.Services.AddScoped(x => httpClient);

            using var serverConfig = await httpClient.GetAsync("api/config/GetConfig");
            await using var stream = await serverConfig.Content.ReadAsStreamAsync();

            //Console.WriteLine(JsonSerializer.Serialize(stream));
            builder.Configuration.AddJsonStream(stream);
            
            builder.Services.AddScoped(sp => 
            {
                var config = sp.GetService<IConfiguration>();
                var result = new ClientConfiguration();
                config.Bind(result);

                return result;
            });

            builder.RootComponents.Add<App>("#app");
            var host = builder.Build();

            // init network service
            var network = host.Services.GetRequiredService<NetworkService>();
            await network.InitAsync();

            //init auth service
            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.Initialize();

            await host.SetDefaultCulture();
            await host.RunAsync();
        }
    }
}
