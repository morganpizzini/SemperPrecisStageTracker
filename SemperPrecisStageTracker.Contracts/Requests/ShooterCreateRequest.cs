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
        public DateTime BirthDate { get; set; } = new(1980, 1, 1);
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        public string FirearmsLicence { get; set; }
        public string BirthLocation { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FiscalCode { get; set; } = string.Empty;

        public DateTime FirearmsLicenceExpireDate { get; set; } = DateTime.Now.AddYears(1);
        public DateTime FirearmsLicenceReleaseDate { get; set; } = DateTime.Now;
        public DateTime? MedicalExaminationExpireDate { get; set; }

    }
}