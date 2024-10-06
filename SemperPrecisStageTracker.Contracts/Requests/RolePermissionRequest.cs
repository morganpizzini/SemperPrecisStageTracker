using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class RolePermissionRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public int PermissionId { get; set; }
}