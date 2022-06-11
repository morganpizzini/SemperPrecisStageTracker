using System.Collections.Generic;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IAuthenticationService
    {
        //ShooterContract User { get; }
        //bool IsAuth { get; }
        Task Initialize();
        Task<bool> Login(string username, string password);
        Task<bool> SignIn(SignInRequest request);
        void UpdateLogin(ShooterContract user);
        void Logout();
        bool CheckPermissions(IPermissionInterface permissions, string resourceId = "");
        bool CheckPermissions(IList<Permissions> permissions, string resourceId = "");
        bool CheckPermissions(Permissions permission, string entityId = "");
    }
}