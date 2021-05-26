using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.Localization;
using SemperPrecisStageTracker.Blazor.Utils;
using Microsoft.Extensions.Configuration;

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
                    options.ValidationMessageLocalizer = ( message, arguments ) =>
                    {
                        var stringLocalizer = options.Services.GetService<IStringLocalizer<Program>>();

                        return stringLocalizer != null && arguments?.Count() > 0
                            ? string.Format( stringLocalizer[message], arguments.ToArray() )
                            : message;
                    };
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services
                .AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.Configuration["baseAddress"])})
                .AddScoped<HttpClientService>();
                //.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.RootComponents.Add<App>("#app");

            var host = builder.Build();
            await host.SetDefaultCulture();
            await host.RunAsync();
        }
    }
}
