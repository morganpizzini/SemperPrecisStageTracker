using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class User : SemperPrecisEntity
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RestorePasswordAlias{get;set;} = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UserData : SemperPrecisEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string FirearmsLicence { get; set; } = string.Empty;
        public DateTime FirearmsLicenceExpireDate { get; set; }
        public DateTime FirearmsLicenceReleaseDate { get; set; }
        public DateTime? MedicalExaminationExpireDate { get; set; }
        public string BirthLocation { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FiscalCode { get; set; } = string.Empty;
        
    }
}