using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SemperPrecisStageTracker.Domain.Services.Mocks
{
    public class KeyVaultServiceMock : IKeyVaultService
    {
        private readonly IConfigurationRoot _configuration;
        public KeyVaultServiceMock(IConfiguration configuration)
        {
            if (configuration is IConfigurationRoot config)
                _configuration = config;
        }
        public Task SetSecret(string secret, string value)
        {
            return Task.CompletedTask;
        }

        public string GetSecret(string secret)
        {
            return _configuration[secret] ?? string.Empty;
        }
    }
}