using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor.Pages
{
    [Authorize]
    public class SemperPrecisBaseComponent : ComponentBase
    {
        [Inject]
        private IHttpService Service { get; set; }
        
        [Inject]
        protected IAuthenticationService AuthService { get; set; }
        
        [Inject]
        protected MainServiceLayer MainServiceLayer { get; set; }

        public bool PageLoading { get; set; } = true;
        public bool ApiLoading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PageLoading = false;
        }

        public Task<T> Post<T>(string uri) => Post<T>(uri,new{ });

        protected async Task<T> Post<T>(string uri, object value)
        {
            ApiLoading = true;
            var result = await Service.Post<T>(uri,value);
            ApiLoading = false;
            return result;
        }
        protected async Task<T> Post<T>(Func<Task<T>> method)
        {
            ApiLoading = true;
            var result = await method();
            ApiLoading = false;
            return result;
        }
    }
}
