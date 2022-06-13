using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class ShooterGroupMoveRequest
{
    [Required]

    public string GroupShooterId { get; set; }
    [Required]

    public string GroupId { get; set; }
        
    public string ShooterName { get; set; }
    public bool ResponseAsGroup { get; set; } = false;
}