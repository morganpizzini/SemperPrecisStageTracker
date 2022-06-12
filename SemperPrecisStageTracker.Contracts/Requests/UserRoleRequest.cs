using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class UserRoleRequest
{
    [Required]
    public string UserRoleId { get; set; } = string.Empty;
}