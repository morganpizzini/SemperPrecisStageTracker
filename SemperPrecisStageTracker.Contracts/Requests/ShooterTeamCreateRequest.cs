using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamCreateRequest
    {
        [Required]

        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required]
        public bool FromShooter { get; set; }
    }
}