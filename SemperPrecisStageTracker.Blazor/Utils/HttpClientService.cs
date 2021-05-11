using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Pages;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public class HttpClientService
    {
        private readonly HttpClient client;
        public HttpClientService(HttpClient client)
        {
            this.client = client;
        }

        public Task<T> PostAsync<T>(string requestUri) => this.PostAsync<T>(requestUri, null);

        public async Task<T> PostAsync<T>(string requestUri, object obj)
        {
            StringContent data = null;
            if (obj != null)
            {
                var json = JsonSerializer.Serialize(obj);
                data = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var result = await client.PostAsync(requestUri, data);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var parse = JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions
                {
                    //PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return parse;
            }
            return default;
        }
    }
}