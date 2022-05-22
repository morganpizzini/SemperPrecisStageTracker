using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Blazor.Services;
using Blazorise;

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

        [Inject]
        private INotificationService NotificationService { get; set; }

        public bool PageLoading { get; set; } = true;
        public bool ApiLoading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PageLoading = false;
        }

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

        protected Task ShowNotification(string message, string title = "", NotificationType notificationType = NotificationType.Info)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Task.CompletedTask;
            }
            switch (notificationType)
            {
                case NotificationType.Warning:
                    return NotificationService.Warning(message, title);
                case NotificationType.Error:
                    return NotificationService.Error(message, title);
                case NotificationType.Success:
                    return NotificationService.Success(message, title);
                case NotificationType.Info:
                default:
                    return NotificationService.Warning(message, title);
            }
        }
    }
}
