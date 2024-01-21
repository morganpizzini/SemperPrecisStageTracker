using Blazorise;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class PresentationalServiceLayer
    {
        private readonly IHttpService _httpService;
        private readonly INotificationService _notificationService;
        public PresentationalServiceLayer(INotificationService notificationService,IHttpService httpService)
        {
            _httpService = httpService;
            _notificationService = notificationService;
        }

        public async Task<(T Result, string Error)> Sample<T>(string uri, object? value = null) where T : new()
        {
            T result = new();

            var apiResponse = await _httpService.Post<T>(uri, value);
            
            if (!apiResponse.WentWell)
            {
                await ShowNotification(apiResponse.Error, "Error in API request", NotificationType.Error);
            }
            else
            {
                result = apiResponse.Result;
            }
            return (result, apiResponse.Error);
        }

        public Task ShowNotification(string message, string title = "", NotificationType notificationType = NotificationType.Info)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Task.CompletedTask;
            }
            return notificationType switch
            {
                NotificationType.Warning => _notificationService.Warning(message, title),
                NotificationType.Error => _notificationService.Error(message, title),
                NotificationType.Success => _notificationService.Success(message, title),
                _ => _notificationService.Warning(message, title),
            };
        }
    }
}