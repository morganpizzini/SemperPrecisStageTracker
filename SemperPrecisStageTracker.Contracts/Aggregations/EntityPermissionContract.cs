using System.Collections.Generic;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Contracts;

public class EntityPermissionContract
{
    public string EntityId { get; set; } = string.Empty;
    public List<Permissions> Permissions { get; set; } = new ();
}