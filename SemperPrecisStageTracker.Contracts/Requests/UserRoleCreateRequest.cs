using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class UserRoleCreateRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public string UserId { get; set; } = string.Empty;
        
    public string EntityId { get; set; } = string.Empty;
}