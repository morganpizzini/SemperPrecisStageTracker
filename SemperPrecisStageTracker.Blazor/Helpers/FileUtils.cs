using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public static class FileUtils
    {
        public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "customFunctions.saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }      
    }
}