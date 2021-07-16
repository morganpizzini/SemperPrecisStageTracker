using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public class NetworkService : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<NetworkService> _dotNetObjectReference;

        public NetworkService(IJSRuntime jsRuntime)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }

        public event EventHandler OnlineChanged;

        public ValueTask InitAsync()
        {
            return _jsRuntime.InvokeVoidAsync("BlazorPWA.init", _dotNetObjectReference);
        }

        [JSInvokable]
        public void UpdateOnlineStatus(bool status)
        {
            OnOnlineChanged(status);
        }

        public bool IsOnline { get; private set; }

        protected void OnOnlineChanged(bool status)
        {
            IsOnline = status;
            OnlineChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _dotNetObjectReference.Dispose();
        }
    }
}