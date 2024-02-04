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
        //Task Initialize();
        void Login(string username, string password,string returnUrl);
        Task<bool> SignIn(SignInRequest request);
        void UpdateLogin(UserContract user);
        void Logout(string returnUrl);
        bool CheckPermissions(IPermissionInterface permissions, string resourceId = "");
        bool CheckPermissions(IList<Permissions> permissions, string resourceId = "");
        bool CheckPermissions(Permissions permission, string entityId = "");
    }
}