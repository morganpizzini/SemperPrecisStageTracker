using System.Collections.Generic;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IAuthenticationService
    {
        //ShooterContract User { get; }
        //bool IsAuth { get; }
        void Initialize();
        Task<bool> Login(string username, string password);
        void UpdateLogin(ShooterContract user);
        void Logout();
        bool CheckPermissions(IList<Permissions> permissions, string resourceId = "");
        bool CheckPermissions(Permissions permission, string entityId = "");
    }
}