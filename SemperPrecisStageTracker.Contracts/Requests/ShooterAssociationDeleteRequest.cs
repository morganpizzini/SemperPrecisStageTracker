using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterAssociationDeleteRequest
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }

    public class ShooterAssociationRequest
    {
        [Required]
        public string ShooterAssociationId { get; set; }
    }
}