using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamPaymentCreateRequest : EntityFilterValidation
    {
        public string EntityId => TeamId;
        [Required]
        public string TeamId { get; set; } = string.Empty;
        [Required]
        public string PaymentTypeId { get; set; } = string.Empty;
        public string ShooterId { get; set; } = string.Empty;
        [Required]
        [Range(0, float.MaxValue, ErrorMessage = $"Please enter valid {nameof(Amount)}")]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime PaymentDateTime { get; set; } = DateTime.Now;
        public DateTime? ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }
    }

    public class PaymentTypeCreateRequest : EntityFilterValidation
    {
        public string EntityId => TeamId;
        [Required]
        public string TeamId { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}