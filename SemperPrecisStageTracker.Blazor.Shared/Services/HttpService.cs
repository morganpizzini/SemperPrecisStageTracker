using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using Fluxor;
using Microsoft.AspNetCore.WebUtilities;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IState<UserState> _userState;
        public HttpService(
            HttpClient httpClient,
            IState<UserState> UserState
        )
        {
            _userState = UserState;
            _httpClient = httpClient;
        }

        public Task<ApiResponse<T>> Get<T>(string uri, Dictionary<string, string>? queryParameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                queryParameters != null ?
                QueryHelpers.AddQueryString(uri, queryParameters) :
                uri);
            return SendRequest<T>(request);
        }

        public Task<ApiResponse<T>> Post<T>(string uri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(value != null ? JsonSerializer.Serialize(value) : "", Encoding.UTF8, "application/json");
            return SendRequest<T>(request);
        }
        public Task<ApiResponse<T>> Put<T>(string uri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(value != null ? JsonSerializer.Serialize(value) : "", Encoding.UTF8, "application/json");
            return SendRequest<T>(request);
        }
        public Task<ApiResponse<T>> Patch<T>(string uri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, uri);
            request.Content = new StringContent(value != null ? JsonSerializer.Serialize(value) : "", Encoding.UTF8, "application/json");
            return SendRequest<T>(request);
        }

        public Task<ApiResponse<T>> Delete<T>(string uri, Dictionary<string, string>? queryParameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, queryParameters != null ?
                QueryHelpers.AddQueryString(uri, queryParameters) :
                uri);
            return SendRequest<T>(request);
        }

        // helper methods

        private async Task<ApiResponse<T>> SendRequest<T>(HttpRequestMessage request)
        {
            // add basic auth header if user is logged in and request is to the api url
            var isApiUrl = !request.RequestUri?.IsAbsoluteUri ?? true;

            if (_userState.Value.User != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _userState.Value.User.AuthData);

            using var response = await _httpClient.SendAsync(request);

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                Dictionary<string, IList<string>> error;
                // auto logout on 401 response
                if (response.StatusCode == HttpStatusCode.Unauthorized && !(request?.RequestUri?.ToString().EndsWith("Authorization/SignIn") ?? false))
                {
                    //_navigationManager.NavigateTo("logout");
                    error = new Dictionary<string, IList<string>>
                            {
                                {"401",new List<string>{"Unauthorized"} }
                            };
                }
                else
                {
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
                        case HttpStatusCode.BadRequest:
                            error = new Dictionary<string, IList<string>>
                            {
                                {"400",new List<string>{"Bad request"} }
                            };
                            break;
                        case HttpStatusCode.InternalServerError:
                        default:
                            error = new Dictionary<string, IList<string>>
                            {
                                {"500",new List<string>{"Server error, please retry later"} }
                            };
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                            break;
                    }
                }
                return new ApiResponse<T>()
                {
                    Error = string.Join(", ", error?.SelectMany(x => x.Value) ?? new List<string> { "Generic error" })
                };
            }
           
            return 
                new ApiResponse<T>()
                {
                    Result = response.StatusCode != HttpStatusCode.NoContent ? (await response.Content.ReadFromJsonAsync<T>())
                                                                             : default(T)
                };
        }
    }
}