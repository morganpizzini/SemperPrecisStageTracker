using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SemperPrecisStageTracker.Blazor.Shared
{
    public partial class CultureSelector
    {
#nullable disable
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
#nullable enable
        CultureInfo[] cultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("it-IT")
        };
        CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    
                    ((IJSInProcessRuntime)JSRuntime).InvokeVoid("blazorCulture.set", value.Name);
                    NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
                }
            }
        }
    }
}
