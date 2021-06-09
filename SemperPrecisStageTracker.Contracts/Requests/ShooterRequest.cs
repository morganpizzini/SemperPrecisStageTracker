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

    public class UserRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string UserId { get; set; }
    }

    public class UserUpdateRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; } = new DateTime(1980,1,1);
        [Required]
        public string Username {get;set;}
        [Required]
        public string Email {get;set;}
    }
}
