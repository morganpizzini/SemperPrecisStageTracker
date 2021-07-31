using System.Threading.Tasks;
using SemperPrecisStageTracker.Domain.Services;

namespace SemperPrecisStageTracker.Mocks.Clients
{
    /// <summary>
    /// Mock implementation for Captcha
    /// </summary>
    public class MockCaptchaValidatorService : ICaptchaValidatorService
    {
        public Task<string> ValidateToken(string token)
        {
            return Task.FromResult(string.Empty);
        }
    }
}