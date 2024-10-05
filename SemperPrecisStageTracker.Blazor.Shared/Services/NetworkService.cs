using Fluxor;
using Microsoft.JSInterop;
using SemperPrecisStageTracker.Blazor.Store;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public class NetworkService : IDisposable
    {
        private readonly IJSInProcessRuntime _jsRuntime;
        private readonly IDispatcher _dispatcher;
        private readonly DotNetObjectReference<NetworkService> _dotNetObjectReference;
        //public event Action<bool> OnlineChanged;


        public NetworkService(IJSRuntime jsRuntime,IDispatcher dispatcher)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _jsRuntime = (IJSInProcessRuntime)jsRuntime;
            _dispatcher = dispatcher;
        }


        public void Init()
        {
            _jsRuntime.InvokeVoid("BlazorPWA.init", _dotNetObjectReference);
        }

        [JSInvokable]
        public void UpdateOnlineStatus(bool status)
        {
            _dispatcher.Dispatch(new SetHasNetworkAction(status));
        }

        //public bool IsOnline { get; private set; }

        //protected void OnOnlineChanged(bool status)
        //{
        //    IsOnline = status;
        //    OnlineChanged?.Invoke(IsOnline);
        //}

        public void Dispose()
        {
            _dotNetObjectReference.Dispose();
        }
    }
}