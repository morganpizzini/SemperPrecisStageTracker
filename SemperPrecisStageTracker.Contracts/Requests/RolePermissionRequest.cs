using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class RolePermissionRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public string PermissionId { get; set; } = string.Empty;
}