using System.Collections.Generic;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

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
        bool CheckPermissions(IEnumerable<string> roles, string resourceId="");
    }

    public class StateService
    {
        public ShooterContract User { get; set; }

        public PermissionsResponse Permissions { get; set; }

        public bool IsAuth => User != null;
    }
}