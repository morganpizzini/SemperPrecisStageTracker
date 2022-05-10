using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Shooter : SemperPrecisEntity
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime BirthDate { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirearmsLicence { get; set; } = string.Empty;
        public DateTime FirearmsLicenceExpireDate { get; set; }
        public DateTime MedicalExaminationExpireDate { get; set; }
    }
}