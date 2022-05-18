using System.Collections.Generic;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class StateService
    {
        public ShooterContract User { get; set; }

        public UserPermissionContract Permissions { get; set; }

        public bool IsAuth => User != null;
    }
}