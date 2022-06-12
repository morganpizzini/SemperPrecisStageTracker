using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class RoleUpdateRequest
{
    [Required]
    public string RoleId { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}