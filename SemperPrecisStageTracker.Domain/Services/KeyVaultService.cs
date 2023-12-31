using Azure.Security.KeyVault.Secrets;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly SecretClient _keyVault;
        public KeyVaultService(SecretClient keyVault)
        {
            _keyVault = keyVault;
        }
        public Task SetSecret(string secret, string value)
        {
            return _keyVault.SetSecretAsync(secret, value);
        }

        public string GetSecret(string secret)
        {
            return _keyVault.GetSecret(secret)?.Value?.Value ?? string.Empty;
        }


    }
}