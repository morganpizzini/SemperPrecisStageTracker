using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace SemperPrecisStageTracker.API.Helpers
{
    public class SampleKeyVaultSecretManager : KeyVaultSecretManager
    {
        public override bool Load(SecretProperties properties) =>
          properties.Enabled.HasValue &&
          properties.Enabled.Value &&
          (!properties.ExpiresOn.HasValue ||
          properties.ExpiresOn.HasValue &&
          properties.ExpiresOn.Value > DateTimeOffset.Now);
    }
}
