using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterCreateRequest
    {
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
        public string FirearmsLicence {get;set;}
        public DateTime FirearmsLicenceExpireDate {get;set;}

    }
}