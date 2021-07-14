using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class EntityFilterValidation
    {
        public virtual string EntityId { get; }
    }

    /// <summary>
    /// Shooter request
    /// </summary>
    public class ShooterRequest : EntityFilterValidation

    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string ShooterId { get; set; }


        public override string EntityId => ShooterId;
    }
}
