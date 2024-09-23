using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserContract
    {
        [IndexDbKey]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompleteName => $"{LastName} {FirstName}";
        // List of "searchable" properties
        public IEnumerable<string> Searchable =>
            (new[] { FirstName?.ToLower() ?? "", LastName?.ToLower() ?? "", Username?.ToLower() ?? "" })
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

        public DateTime BirthDate { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string AuthData { get; set; }
        public bool IsActive { get;set;} = false;
        [IndexDbIndex]
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirearmsLicence { get; set; }
        public DateTime FirearmsLicenceExpireDate { get; set; } = DateTime.Now;
        public DateTime FirearmsLicenceReleaseDate { get; set; } = DateTime.Now;
        public DateTime? MedicalExaminationExpireDate { get; set; }
        public string BirthLocation { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CompleteAddress => $"{Address}, {City} ({PostalCode}), {Region} - {Country}";
        public string Phone { get; set; } = string.Empty;
        public string FiscalCode { get; set; } = string.Empty;
        // set when personal data is not provided
        public bool? Warning { get; set; }
        public bool HasWarning => Warning ?? (Warning = this.CalculateWarning(FirearmsLicenceExpireDate, MedicalExaminationExpireDate)).Value;

        public IList<UserAssociationContract> Classifications { get; set; } = new List<UserAssociationContract>();
        public IList<TeamContract> Teams { get; set; } = new List<TeamContract>();
    }

    public static class ContractExtension
    {
        public static bool CalculateWarning(this UserContract _, DateTime firearmsLicenceExpireDate, DateTime? medicalExaminationExpireDate)
        =>
            firearmsLicenceExpireDate.Date <= DateTime.Now ||
                                  !medicalExaminationExpireDate.HasValue || medicalExaminationExpireDate.Value.Date <= DateTime.Now;
    }
}