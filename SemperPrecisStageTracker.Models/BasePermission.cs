using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    /// <summary>
    /// edit-match
    /// edit-shooter
    /// view-shooter
    /// </summary>
    public class Permission : SemperPrecisEntity
    {
        public string Name { get; set; }
    }

    public class PermissionRole : SemperPrecisEntity
    {
        public string PermissionId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
    }

    /// <summary>
    /// match-director
    /// SO
    /// team-manager
    /// </summary>
    public class Role : SemperPrecisEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

     /// <summary>
    /// user1 -> SO match 1
    /// user2 -> team-manager "Team1"
    /// </summary>
    public class UserRole : SemperPrecisEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }

    /// <summary>
    /// user1 -> match-edit match 1
    /// user2 -> team-shooter-manage "Team3"
    /// </summary>
    public class UserPermission : SemperPrecisEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// admin -> all user in this group has admin premission
    /// MatchManager -> all user in this group can manage matched
    /// ShooterManager -> all user in this group can manage shooters
    /// </summary>
    public class PermissionGroup : SemperPrecisEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UserPermissionGroup : SemperPrecisEntity
    {
        public string PermissionGroupId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class PermissionGroupRole : SemperPrecisEntity
    {
        public string GroupId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
    }
}