using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class PermissionsResponse
    {
        public IList<AdministrationPermissionContract> AdministrationPermissions { get; set; } =
            new List<AdministrationPermissionContract>();
        public IList<EntityPermissionContract> EntityPermissions { get; set; } = new List<EntityPermissionContract>();
    }
}