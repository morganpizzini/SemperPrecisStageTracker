using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class RoleRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
}