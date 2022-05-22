using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SemperPrecisStageTracker.Blazor.Services.Models;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        //private ILocalStorageService _localStorageService;
        private readonly StateService _stateService;
        
        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            StateService stateService
        //ILocalStorageService localStorageService
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _stateService = stateService;
            //_localStorageService = localStorageService;
        }

        public async Task<ApiResponse<T>> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public Task<ApiResponse<T>> Post<T>(string uri) => Post<T>(uri, new { });

        public async Task<ApiResponse<T>> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }

        // helper methods

        private async Task<ApiResponse<T>> sendRequest<T>(HttpRequestMessage request)
        {
            // add basic auth header if user is logged in and request is to the api url
            //var user = _authenticationService.User
            var isApiUrl = !request.RequestUri.IsAbsoluteUri;
            if (_stateService.IsAuth && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _stateService.User.AuthData);

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized && !request.RequestUri.ToString().EndsWith("Authorization/SignIn"))
            {
                _navigationManager.NavigateTo("app/logout");
                return default;
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, IList<string>>>();
                
                return new ApiResponse<T>()
                {
                    Error = string.Join(", ",error?.SelectMany(x=>x.Value) ?? new List<string>{"Generic error"})
                };
            }
            return new ApiResponse<T>()
            {
                Result = await response.Content.ReadFromJsonAsync<T>()
            };
        }
    }
}