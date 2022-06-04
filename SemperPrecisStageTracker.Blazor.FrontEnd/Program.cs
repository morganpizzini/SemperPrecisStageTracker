using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using SemperPrecisStageTracker.Blazor;
using SemperPrecisStageTracker.Blazor.FrontEnd;
using SemperPrecisStageTracker.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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

builder.Services
    .AddScoped<StateService>()
    .AddScoped<IHttpService, HttpService>();

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

using var serverConfig = new HttpRequestMessage(HttpMethod.Get, "api/config/GetConfig");
using var responseConfig = await httpClient.SendAsync(serverConfig);

if (responseConfig.IsSuccessStatusCode)
{
    await using var stream = await responseConfig.Content.ReadAsStreamAsync();

    builder.Configuration.AddJsonStream(stream);
}

builder.Services.AddScoped(sp =>
{
    var config = sp.GetService<IConfiguration>();
    var result = new ClientConfiguration();
    config.Bind(result);

    return result;
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
