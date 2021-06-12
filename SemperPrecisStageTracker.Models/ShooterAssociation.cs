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

        public bool ShooterOfficier {get; set;}
        [Required]
        public string Classification { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}