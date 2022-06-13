using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterTeam : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public bool ShooterApprove { get; set; }
        [Required]
        public bool TeamApprove { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}