using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterTeamPayment : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime PaymentDateTime { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool NotifyExpiration { get; set; }
    }
}