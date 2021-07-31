using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class StateService
    {
        public ShooterContract User { get; set; }

        public PermissionsResponse Permissions { get; set; }

        public bool IsAuth => User != null;
    }
}