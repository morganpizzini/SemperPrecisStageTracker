using System.Collections.Generic;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Contracts;

public class UserPermissionContract
{
    public List<Permissions> GenericPermissions { get; set; } = new ();
    public List<EntityPermissionContract> EntityPermissions { get; set; } = new ();
}