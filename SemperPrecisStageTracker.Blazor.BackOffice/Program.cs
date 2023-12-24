using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SemperPrecisStageTracker.Blazor;
using SemperPrecisStageTracker.Blazor.BackOffice;
using SemperPrecisStageTracker.Blazor.Components;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
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

var baseAddress = builder.Configuration["baseAddress"];
if (string.IsNullOrEmpty(baseAddress))
    throw new NullReferenceException("baseAddress configuration is not specified");

var apiUrl = new Uri(baseAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUrl });


await builder.Build().RunAsync();
