using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class TeamPayment : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public string PaymentType { get; set; } = string.Empty;
        [Required]
        public DateTime PaymentDateTime { get; set; }
    }

    public class PaymentType : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }

    public class TeamReminder : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        public string UserId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }
    }
}