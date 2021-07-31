using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class CaptchaResponse
    {
        public bool Success { get; set; }
        [JsonPropertyName("error-codes")]
        public IList<string> ErrorCodes { get; set; }
    }
}