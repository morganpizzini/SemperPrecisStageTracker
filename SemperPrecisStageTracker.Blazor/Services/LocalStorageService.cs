using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

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

        /// <summary>
        /// https://dbushell.com/2020/06/08/pwa-web-crypto-encryption-auto-sign-in-redux-persist/
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ValueTask<string> EncodeSecret(string key,string name,string value)
        {
            return _jsRuntime.InvokeAsync<string>("encrFunctions.encrypt", key,name,value);
        }

        public ValueTask<string> DecodeSecret(string key,string name)
        {
            return _jsRuntime.InvokeAsync<string>("encrFunctions.decrypt", key, name);

        }
    }
}