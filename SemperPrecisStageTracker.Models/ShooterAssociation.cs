using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class ShooterAssociationInfo : SemperPrecisEntity
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        public IList<string> Categories { get; set; } = new List<string>();
        public bool SafetyOfficier { get; set; }
        public DateTime RegistrationDate { get; set; }
        [Required]
        public string CardNumber { get; set; } = string.Empty;
    }

    public class ShooterAssociation : SemperPrecisEntity
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
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}