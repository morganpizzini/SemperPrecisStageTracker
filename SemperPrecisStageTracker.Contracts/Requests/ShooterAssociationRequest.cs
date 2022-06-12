using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests;

public class ShooterAssociationRequest
{
    [Required]
    public string ShooterAssociationId { get; set; }
}