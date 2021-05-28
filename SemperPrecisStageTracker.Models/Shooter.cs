using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Shooter : SemperPrecisEntity
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public string Username {get;set;}
        public string Email {get;set;}
        public string Password {get;set;}
    }
    public class ShooterTeam : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
    public class ShooterAssociation : SemperPrecisEntity
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string Rank { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}