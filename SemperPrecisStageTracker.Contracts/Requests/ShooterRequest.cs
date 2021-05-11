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
        public DateTime BirthDate { get; set; }
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

}
