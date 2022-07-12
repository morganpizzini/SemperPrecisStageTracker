using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamPaymentRequest : EntityFilterValidation
    {
        public override string EntityId => TeamId;
        [Required]
        public string ShooterTeamPaymentId { get; set; }
        [Required]
        public string TeamId { get; set; }
    }

    public class PaymentTypeRequest : EntityFilterValidation
    {
        public override string EntityId => TeamId;
        [Required]
        public string PaymentTypeId { get; set; }
        [Required]
        public string TeamId { get; set; }
    }
}