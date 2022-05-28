using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Group request
    /// </summary>
    public class GroupRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string GroupId { get; set; }
    }

    
    public class RoleRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
    }

    public class RolePermissionRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
        [Required]
        public string PermissionId { get; set; } = string.Empty;
    }

    public class RolePermissionCreateRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
        [Required]
        public string PermissionId { get; set; } = string.Empty;
        
        //public string EntityId { get; set; } = string.Empty;
    }

    public class RoleCreateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }

    public class RoleUpdateRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
