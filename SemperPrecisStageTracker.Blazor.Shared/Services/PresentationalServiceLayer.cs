using Blazorise;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Contracts.Requests;

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

        public async Task<BaseResponse<T>> CallRestfull<T>(RequestType requestType, string uri, Dictionary<string, string>? queryParameters, object? body = null,string positiveMessage = "")
        {
            ApiResponse<BaseResponse<T>> response;
            BaseResponse<T> result = new();
            switch (requestType)
            {
                case RequestType.Get:
                    response = await _httpService.Get<BaseResponse<T>>(uri, queryParameters);
                    break;
                case RequestType.Post:
                    response = await _httpService.Post<BaseResponse<T>>(uri, body);
                    break;
                case RequestType.Put:
                    response = await _httpService.Put<BaseResponse<T>>(uri, body);
                    break;
                case RequestType.Patch:
                    response = await _httpService.Patch<BaseResponse<T>>(uri, body);
                    break;
                case RequestType.Delete:
                    response = await _httpService.Delete<BaseResponse<T>>(uri);
                    break;
                default:
                    return new BaseResponse<T>();
            };

            if (!response.WentWell)
            {
                await ShowNotification(response.Error, "Error in API request", NotificationType.Error);
                result.Errors.Add(response.Error, new[] { "Error in API request" });
            }
            else
            {
                if(!string.IsNullOrEmpty(positiveMessage))
                {
                    await ShowNotification(positiveMessage, "Success", NotificationType.Success);
                }
                if(response.Result != null)
                    result = response.Result;
            }
            return result;
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