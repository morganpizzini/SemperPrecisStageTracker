using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using SemperPrecisStageTracker.Blazor.Pages;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    //public class HttpClientService
    //{
    //    private readonly HttpClient client;
    //    public HttpService(HttpClient client)
    //    {
    //        this.client = client;
    //    }

    //    public Task<T> Post<T>(string requestUri) => this.Post<T>(requestUri, null);

    //    public async Task<T> Post<T>(string requestUri, object obj)
    //    {
    //        StringContent data = null;
    //        if (obj != null)
    //        {
    //            var json = JsonSerializer.Serialize(obj);
    //            data = new StringContent(json, Encoding.UTF8, "application/json");
    //        }
    //        var result = await ExecuteAsync(http => http.Post(requestUri, data));
    //        // var result = await client.Post(requestUri, data);

    //        if (result!= null && result.IsSuccessStatusCode)
    //        {
    //            var response = await result.Content.ReadAsStringAsync();
    //            var parse = JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions
    //            {
    //                //PropertyNameCaseInsensitive = true,
    //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //            });
    //            return parse;
    //        }
    //        return default;
    //    }

    //    private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> httpCall)
    //    {
    //        try
    //        {
    //            return await httpCall(client);
    //        }
    //        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            Console.WriteLine($"Error intercepted {e.StatusCode}");

    //            // var parameters = new ModalParameters();
    //            // parameters.Add(nameof(SimpleMessage.Message), 
    //            // "Il server ha restituito un errore, si consiglia di riprovare più tardi");

    //            // _modal.Show<SimpleMessage>("Errore durante la chiamata di rete", parameters);

    //            return default;
    //        }
    //    }
    //}
}