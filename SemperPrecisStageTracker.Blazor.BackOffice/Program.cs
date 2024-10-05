using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SemperPrecisStageTracker.Blazor;
using SemperPrecisStageTracker.Blazor.BackOffice;
using SemperPrecisStageTracker.Blazor.BackOffice.Services;
using SemperPrecisStageTracker.Blazor.Components;
using SemperPrecisStageTracker.Blazor.Services;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Shared.Cache;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddAuthorizationCore();

builder.Services.AddSingleton<ISemperPrecisMemoryCache, BlazorMemoryCache>()
                .AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
                .AddScoped<IAuthorizationHandler, PermissionServiceHandler>();

var currentAssembly = typeof(UserState).Assembly;
builder.Services.AddFluxor(options => {
    options.ScanAssemblies(currentAssembly);

#if DEBUG
    options.UseReduxDevTools(rdt =>
    {
        rdt.EnableStackTrace();
    });
#endif
});

if (builder.HostEnvironment.IsDevelopment())
{
    builder.Services.AddScoped<IHttpService, MockHttpService>();
}
else
{
    builder.Services.AddScoped<IHttpService, HttpService>();
}

builder.Services
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<ILocalStorageService, LocalStorageService>()
    .AddScoped<MainServiceLayer>()
    .AddScoped<NetworkService>()
    .AddScoped<PresentationalServiceLayer>();

var baseAddress = builder.Configuration["baseAddress"];
if (string.IsNullOrEmpty(baseAddress))
    throw new NullReferenceException("baseAddress configuration is not specified");

//var apiUrl = new Uri(baseAddress);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUrl });

var host = builder.Build();

// init Main service
var main = host.Services.GetRequiredService<MainServiceLayer>();
await main.Init();

await host.RunAsync();