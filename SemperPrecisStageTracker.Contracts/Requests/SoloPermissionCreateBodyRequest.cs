using SemperPrecisStageTracker.Shared.Permissions;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class SoloPermissionCreateBodyRequest
    {
        public HashSet<Permissions> Permissions { get; set; } = new HashSet<Permissions>();
    }
}
