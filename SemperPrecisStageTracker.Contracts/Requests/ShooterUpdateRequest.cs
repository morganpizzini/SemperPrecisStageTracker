using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
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
        [Required]
        public string Username {get;set;}
        [Required]
        public string Email {get;set;}
    }

    /// <summary>
    /// Update user password request
    /// </summary>
    public class UserPasswordUpdateRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// User new password
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}