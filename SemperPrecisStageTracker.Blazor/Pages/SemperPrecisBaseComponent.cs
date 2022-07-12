using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Pages
{
    public class SemperPrecisComponent : SemperPrecisBasePresentationalComponent
    {
        [Inject]
        protected IHttpService Service { get; set; }

        public Task<T> Post<T>(string uri) where T : new() => Post<T>(uri, new { });


        protected virtual Task<string> Post1(string uri, object value, bool pageOperation = true)
        {
            return Task.FromResult(string.Empty);
        }

        protected virtual async Task<T> Post<T>(string uri, object value, bool pageOperation = true) where T : new()
        {
            if (pageOperation)
                ApiLoading = true;
            T result = new();

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

    [Authorize]
    public class SemperPrecisBaseComponent : SemperPrecisComponent
    {
        [Inject]
        protected MainServiceLayer MainServiceLayer { get; set; }
    }

    public class SemperPrecisBaseComponent<T> : SemperPrecisBaseComponent where T : new()
    {
        protected T Model = new();

        protected override async Task<string> Post1(string uri, object value, bool pageOperation = true)
        {
            if (pageOperation)
                ApiLoading = true;

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
                    Model = apiResponse.Result;
                }
            }
            if (pageOperation)
                ApiLoading = false;

            return apiResponse?.Error ?? string.Empty;
        }
    }
}
