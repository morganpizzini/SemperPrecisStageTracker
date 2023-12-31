using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Domain.Services
{
    public interface IKeyVaultService
    {
        Task SetSecret(string secret, string value);
        string GetSecret(string secret);
    }
}