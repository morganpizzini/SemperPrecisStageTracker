using Microsoft.JSInterop;
using System.Text.Json;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private IJSInProcessRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = (IJSInProcessRuntime)jsRuntime;
        }

        public T GetItem<T>(string key)
        {
            var json = _jsRuntime.Invoke<string>("localStorage.getItem", key);

            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public void SetItem<T>(string key, T value)
        {
            _jsRuntime.InvokeVoid("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        public void RemoveItem(string key)
        {
            _jsRuntime.InvokeVoid("localStorage.removeItem", key);
        }
    }
}