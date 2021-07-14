using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterUpdateRequest: ShooterRequest
    {
        //[Required]
        //public string ShooterId { get; set; }
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
}