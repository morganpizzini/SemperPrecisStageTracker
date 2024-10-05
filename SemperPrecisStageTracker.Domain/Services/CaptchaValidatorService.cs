using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class CaptchaValidatorService : ICaptchaValidatorService
    {
        private readonly HttpClient _httpClient;
        private readonly string recaptchaToken;
        private readonly string url;

        public CaptchaValidatorService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            recaptchaToken = configuration["recaptchaSiteKey"];
            url = configuration["recaptchaSiteAuthority"];
        }

        public async Task<string> ValidateToken(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{url}?secret={recaptchaToken}&response={token}")
            {
                //Content = new StringContent(
                //    JsonSerializer.Serialize(new {secret = recaptchaToken, response = token}), Encoding.UTF8,
                //    "application/json")
            };

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return "Cannot verify token";
            }

            var parsedData = await response.Content.ReadFromJsonAsync<CaptchaResponse>();

            if (parsedData == null)
                return "Error on response";

            return parsedData is { Success: true } ? string.Empty : string.Join(", ", parsedData.ErrorCodes);
        }
    }
}