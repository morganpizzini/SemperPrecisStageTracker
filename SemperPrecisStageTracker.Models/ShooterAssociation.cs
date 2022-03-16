using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterAssociation : SemperPrecisEntity
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }

        public string CardNumber { get; set; }

        public bool SafetyOfficier {get; set;}
        [Required]
        public string Classification { get; set; }
        [Required]
        public string Division { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}