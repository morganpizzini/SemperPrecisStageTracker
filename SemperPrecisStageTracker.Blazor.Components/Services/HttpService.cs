using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using Fluxor;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        //private ILocalStorageService _localStorageService;
        //private readonly StateService _stateService;
        private readonly IState<UserState> _userState;
        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            IState<UserState> UserState
            //StateService stateService
        //ILocalStorageService localStorageService
        )
        {
            _userState = UserState;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            //_stateService = stateService;
            //_localStorageService = localStorageService;
        }

        public async Task<ApiResponse<T>?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public Task<ApiResponse<T>?> Post<T>(string uri) => Post<T>(uri, new { });

        public async Task<ApiResponse<T>?> Post<T>(string uri, object? value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequest<T>(request);
        }

        // helper methods

        private async Task<ApiResponse<T>?> SendRequest<T>(HttpRequestMessage request)
        {
            // add basic auth header if user is logged in and request is to the api url
            var isApiUrl = !request.RequestUri?.IsAbsoluteUri ?? true;

            if (_userState.Value.User != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _userState.Value.User.AuthData);

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized && !(request?.RequestUri?.ToString().EndsWith("Authorization/SignIn") ?? false))
            {
                //_navigationManager.NavigateTo("logout");
                return default;
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                Dictionary<string, IList<string>> error;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        error = new Dictionary<string, IList<string>>
                        {
                            {"401",new List<string>{"Unauthorized"} }
                        };
                        break;
                    case HttpStatusCode.NotFound:
                        error = new Dictionary<string, IList<string>>
                        {
                            {"404",new List<string>{"Endpoint not found"} }
                        };
                        break;
                    default:
                        error = await response.Content.ReadFromJsonAsync<Dictionary<string, IList<string>>>();
                        break;
                }
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