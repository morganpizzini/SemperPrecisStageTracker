using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamPaymentUpdateRequest : EntityFilterValidation
    {
        public override string EntityId => TeamId;
        [Required]
        public string ShooterTeamPaymentId { get; set; }
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string PaymentTypeId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        [Range(0, float.MaxValue, ErrorMessage = $"Please enter valid {nameof(Amount)}")]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime PaymentDateTime { get; set; }
    }

        public class PaymentTypeUpdateRequest : EntityFilterValidation
    {
        public override string EntityId => TeamId;
        [Required]
        public string PaymentTypeId { get; set; }
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}