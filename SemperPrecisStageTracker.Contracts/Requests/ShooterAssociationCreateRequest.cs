using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterAssociationCreateRequest
    {
        [Required]

        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string Classification { get; set; }
        [Required]
        public string Division { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}