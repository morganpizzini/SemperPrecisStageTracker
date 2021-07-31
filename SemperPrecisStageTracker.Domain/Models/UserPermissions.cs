using System.Collections.Generic;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Domain.Models
{
    public class UserPermissions
    {
        public ICollection<AdministrationPermissions> AdministratorPermissions { get; set; } =
            new List<AdministrationPermissions>();

        public IDictionary<string, List<EntityPermissions>> EntityPermissions { get; set; } =
            new Dictionary<string, List<EntityPermissions>>();
    }
}
