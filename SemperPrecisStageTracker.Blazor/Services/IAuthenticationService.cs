using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IAuthenticationService
    {
        ShooterContract User { get; }
        bool IsAuth { get; }
        Task Initialize();
        Task<bool> Login(string username, string password);
        Task UpdateLogin(ShooterContract user);
        Task Logout();
    }
}