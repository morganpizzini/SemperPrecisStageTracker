using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class GroupShooterRequest
{
    [Required]
    public string GroupShooterId { get; set; }
}