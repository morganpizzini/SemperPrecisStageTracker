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
        public static void SetDefaultCulture(this WebAssemblyHost host)
        {
            var jsInterop = (IJSInProcessRuntime)host.Services.GetRequiredService<IJSRuntime>();
            var result = jsInterop.Invoke<string>("blazorCulture.get");

            var culture = result != null ? new CultureInfo(result) : new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
