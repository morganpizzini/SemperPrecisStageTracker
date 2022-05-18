using System.Collections.Generic;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Domain.Models
{
    public class UserPermissions
    {
        public IDictionary<string, List<Permissions>> Permissions { get; set; } =
            new Dictionary<string, List<Permissions>>();
    }
}
