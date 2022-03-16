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
    }
    public class TeamHolder : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    public class GroupShooter : SemperPrecisEntity
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string DivisionId {get; set;}
        [Required]
        public string TeamId {get; set;}
    }
}