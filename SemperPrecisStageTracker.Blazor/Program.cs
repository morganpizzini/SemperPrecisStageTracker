using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using DnetIndexedDb;
using DnetIndexedDb.Models;
using Microsoft.Extensions.Localization;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Contracts;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services.IndexDB;
using Blazorise.Bootstrap5;

namespace SemperPrecisStageTracker.Blazor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddBlazorise(options =>
            {
                options.Immediate = true;
                //options.ChangeTextOnKeyPress = true;
                options.ValidationMessageLocalizer = (message, arguments) =>
                {
                    var stringLocalizer = options.Services.GetService<IStringLocalizer<Program>>();

                    var trimSpace = message.IndexOf('|');
                    if (trimSpace > 0)
                    {
                        message = message.Substring(0, trimSpace);
                    }
                    return stringLocalizer != null
                        ? string.Format(stringLocalizer[message], arguments?.ToArray() ?? new string[0])
                        : message;
                };
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
                        .AddScoped<StateService>()
                        .AddScoped<IAuthorizationHandler, PermissionHandler>();



        builder.Services
            .AddScoped<IHttpService, HttpService>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ILocalStorageService, LocalStorageService>()
            .AddScoped<NetworkService>()
            .AddScoped<MainServiceLayer>();

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

        using var serverConfig = new HttpRequestMessage(HttpMethod.Get, "api/config/GetConfig");
        using var responseConfig = await httpClient.SendAsync(serverConfig);

        await using var stream = await responseConfig.Content.ReadAsStreamAsync();

        builder.Configuration.AddJsonStream(stream);

        builder.Services.AddScoped(sp =>
        {
            var config = sp.GetService<IConfiguration>();
            var result = new ClientConfiguration();
            config.Bind(result);

            return result;
        });

        // index db
        builder.Services.AddIndexedDbDatabase<MatchServiceIndexedDb>(options =>
        {
            var indexedDbDatabaseModel = new IndexedDbDatabaseModel()
                .WithName("OfflineContent")
                .WithVersion(1);

            indexedDbDatabaseModel.AddStore<MatchContract>();
            //indexedDbDatabaseModel.AddStore<ShooterContract>();
            //indexedDbDatabaseModel.AddStore<StageContract>();
            //indexedDbDatabaseModel.AddStore<GroupContract>();
            indexedDbDatabaseModel.AddStore<ShooterStageAggregationResult>();
            indexedDbDatabaseModel.AddStore<ShooterMatchContract>();
            indexedDbDatabaseModel.AddStore<ShooterSOStageContract>();
            indexedDbDatabaseModel.AddStore<EditedEntity>();

            // add Offline settings
            // clean/download all method
            // services for avoid api call and use indexDB
            // services for push all
            // api services for update edited entities

            options.UseDatabase(indexedDbDatabaseModel);
        });


        builder.RootComponents.Add<App>("#app");
        var host = builder.Build();

        // init network service
        var network = host.Services.GetRequiredService<NetworkService>();
        network.Init();

        //init auth service
        var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
        authenticationService.Initialize();

        host.SetDefaultCulture();
        await host.RunAsync();
    }
}
