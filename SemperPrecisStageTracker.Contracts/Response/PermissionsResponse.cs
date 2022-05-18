using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class PermissionsResponse
    {
        public IList<PermissionContract> Permissions { get; set; } =
            new List<PermissionContract>();
    }
}