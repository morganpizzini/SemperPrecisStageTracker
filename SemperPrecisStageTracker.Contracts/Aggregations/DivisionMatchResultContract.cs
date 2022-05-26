using System.Collections.Generic;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Contracts
{
    public class DivisionMatchResultContract
    {
        public string Name { get; set; }
        public IList<ShooterClassificationResultContract> Classifications { get; set; } = new List<ShooterClassificationResultContract>();
    }

    public class UserPermissionContract
    {
        public List<Permissions> GenericPermissions { get; set; } = new ();
        public List<EntityPermissionContract> EntityPermissions { get; set; } = new ();
    }

    
    public class EntityPermissionContract
    {
        public string EntityId { get; set; } = string.Empty;
        public List<Permissions> Permissions { get; set; } = new ();
    }
}