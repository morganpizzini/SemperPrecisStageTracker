using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterAssociationDeleteRequest
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }

    public class ShooterAssociationRequest
    {
        [Required]
        public string ShooterAssociationId { get; set; }
    }

    public class ShooterAssociationInfoCreateRequest
    {
        [Required]
        public string AssociationId { get; set;}
        [Required]
        public string ShooterId { get; set;}
        public IList<string> Categories { get; set; } = new List<string>();
        public string CardNumber { get; set; }
        public DateTime RegistrationDate { get; set; }  = DateTime.Now;
        public bool SafetyOfficier { get; set; }
    }

    public class ShooterAssociationInfoRequest
    {
        public string ShooterAssociationInfoId { get; set; }
    }

        public class ShooterAssociationInfoUpdateRequest
    {
        [Required]
        public string ShooterAssociationInfoId { get; set; }
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        public IList<string> Categories { get; set; } = new List<string>();
        public string CardNumber { get; set; }
        public DateTime RegistrationDate { get; set; }  = DateTime.Now;
        public bool SafetyOfficier { get; set; }
    }
}