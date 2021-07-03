using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Domain.Services
{
    public interface ICaptchaValidatorService
    {
        Task<string> ValidateToken(string token);
    }
}