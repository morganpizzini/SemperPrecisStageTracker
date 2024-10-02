using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
using Microsoft.AspNetCore.Components.Web;
using SemperPrecisStageTracker.Blazor;
using Fluxor;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using SemperPrecisStageTracker.Blazor.Components;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.FeatureManagement;
using SemperPrecisStageTracker.Shared.Cache;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFeatureManagement();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddBlazorise(options =>
    {
        options.Debounce = true;
        options.DebounceInterval = 300;
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
                ? string.Format(stringLocalizer[message], arguments?.ToArray() ?? [])
                : message;
        };
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services
    .AddSingleton<ISemperPrecisMemoryCache, BlazorMemoryCache>()
    .AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
                .AddScoped<IAuthorizationHandler, PermissionServiceHandler>();

var currentAssembly = typeof(UserState).Assembly;
builder.Services.AddFluxor(options =>{ 
    options.ScanAssemblies(currentAssembly);

#if DEBUG
    options.UseReduxDevTools(rdt =>
    {
        rdt.EnableStackTrace();
    });
#endif
});

builder.Services
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<ILocalStorageService, LocalStorageService>()
    .AddScoped<NetworkService>()
    .AddScoped<MainServiceLayer>()
    .AddScoped<PresentationalServiceLayer>();

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

var baseAddress = builder.Configuration["baseAddress"];
if (string.IsNullOrEmpty(baseAddress))
    throw new NullReferenceException("baseAddress configuration is not specified");

var apiUrl = new Uri(baseAddress);

// use fake backend if "fakeBackend" is "true" in appsettings.json
// if (builder.Configuration["fakeBackend"] == "true")
//     return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

var httpClient = new HttpClient() { BaseAddress = apiUrl };

//configure http client
builder.Services.AddScoped(x => httpClient);
try
{
    using var serverConfig = new HttpRequestMessage(HttpMethod.Get, "api/config/GetConfig");
    using var responseConfig = await httpClient.SendAsync(serverConfig);

    if (responseConfig.IsSuccessStatusCode)
    {
        await using var stream = await responseConfig.Content.ReadAsStreamAsync();

        builder.Configuration.AddJsonStream(stream);
    }
}catch{}

builder.Services.AddScoped(sp =>
{
    var config = sp.GetService<IConfiguration>();
    var result = new ClientConfiguration();
    if(config != null)
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
    indexedDbDatabaseModel.AddStore<UserStageAggregationResult>();
    indexedDbDatabaseModel.AddStore<UserMatchContract>();
    indexedDbDatabaseModel.AddStore<UserSOStageContract>();
    indexedDbDatabaseModel.AddStore<EditedEntity>();
    
    options.UseDatabase(indexedDbDatabaseModel);
});


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var host = builder.Build();

//init auth service
//var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
//await authenticationService.Initialize().ConfigureAwait(false);

// init network service
var network = host.Services.GetRequiredService<NetworkService>();
network.Init();
network.Dispose();
// init Main service
var main = host.Services.GetRequiredService<MainServiceLayer>();
await main.Init().ConfigureAwait(false);

host.SetDefaultCulture();
await host.RunAsync();
