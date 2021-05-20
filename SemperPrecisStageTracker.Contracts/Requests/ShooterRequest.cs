using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Shooter request
    /// </summary>
    public class ShooterRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string ShooterId { get; set; }
       
    }

    public class ShooterCreateRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; } = new DateTime(1980,01,01);
    }

    public class ShooterUpdateRequest
    {
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
    }
 public class ShooterTeamCreateRequest
    {
        [Required]

        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
    public class ShooterTeamDeleteRequest
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }

     public class ShooterAssociationCreateRequest
    {
        [Required]

        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string Rank { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
    public class ShooterAssociationDeleteRequest
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }
}
