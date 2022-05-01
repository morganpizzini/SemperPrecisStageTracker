using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public static class EventExtensions
    {
        public static async Task DebounceEvent(this IJSRuntime jsRuntime, ElementReference element, string eventName, TimeSpan delay)
        {
            await using var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/events.js");
            await module.InvokeVoidAsync("debounceEvent", element, eventName, (long)delay.TotalMilliseconds);
        }

        public static async Task ThrottleEvent(this IJSRuntime jsRuntime, ElementReference element, string eventName, TimeSpan delay)
        {
            await using var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/events.js");
            await module.InvokeVoidAsync("throttleEvent", element, eventName, (long)delay.TotalMilliseconds);
        }
    }
}