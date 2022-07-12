using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Shooter and Team request
    /// </summary>
    public class ShooterTeamRequest : EntityFilterValidation
    {
        public override string EntityId => TeamId;
        /// <summary>
        /// Shooter identifier
        /// </summary>
        [Required]
        public string ShooterId { get; set; }

        /// <summary>
        /// Team identifier
        /// </summary>
        [Required]
        public string TeamId { get; set; }

    }
}
