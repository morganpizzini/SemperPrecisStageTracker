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
        Task Initialize();
        Task<bool> Login(string username, string password);
        Task UpdateLogin(ShooterContract user);
        Task Logout();
        bool CheckPermissions(IList<Permissions> permissions, string resourceId = "");
        bool CheckPermissions(Permissions permission, string entityId = "");
    }
}