using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public static class FileUtils
    {
        public static void SaveAs(this IJSRuntime js, string filename, byte[] data)
        {
            ((IJSInProcessRuntime)js).Invoke<object>(
                "customFunctions.saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }
    }
}