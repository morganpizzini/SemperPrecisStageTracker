using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts;

public class RoleContract
{
    public string RoleId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IList<PermissionContract> Permissions { get; set; } = new List<PermissionContract>();
    public IList<UserRoleContract> UserRoles { get; set; } = new List<UserRoleContract>();
}