using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;
using Blazorise;
using Microsoft.AspNetCore.Authorization;

namespace SemperPrecisStageTracker.Blazor.Pages
{
    [Authorize]
    public class SemperPrecisBaseComponent : SemperPrecisBasePresentationalComponent
    {
        [Inject]
        private IHttpService Service { get; set; }
        
        [Inject]
        protected MainServiceLayer MainServiceLayer { get; set; }
        
        public Task<T> Post<T>(string uri) => Post<T>(uri, new { });

        protected async Task<T> Post<T>(string uri, object value)
        {
            ApiLoading = true;
            T result = default;
            
                var apiResponse = await Service.Post<T>(uri, value);
                if (apiResponse == null)
                {
                    await ShowNotification("Please retry in a while", "Generic error", NotificationType.Error);
                    
                }
                else
                {
                    if (!string.IsNullOrEmpty(apiResponse.Error))
                    {
                        await ShowNotification(apiResponse.Error, "Error in API request", NotificationType.Error);
                    }
                    else
                    {
                        result = apiResponse.Result;
                    }
                }
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
