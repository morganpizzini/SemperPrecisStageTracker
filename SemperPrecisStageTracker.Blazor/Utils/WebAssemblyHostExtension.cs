using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public static class WebAssemblyHostExtension
    {
        public static async Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");

            var culture = result != null ? new CultureInfo(result) : new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
