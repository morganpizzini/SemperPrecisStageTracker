﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterContract
    {
        [IndexDbKey]
        public string ShooterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompleteName => $"{LastName} {FirstName}";
        // List of "searchable" properties
        public IEnumerable<string> Searchable =>
            (new[] { FirstName?.ToLower() ?? "", LastName?.ToLower() ?? "", Username?.ToLower() ?? "" })
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

        public DateTime BirthDate { get; set; }
        public string AuthData { get; set; }
        [IndexDbIndex]
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirearmsLicence { get; set; }
        public DateTime FirearmsLicenceExpireDate { get; set; }
        public DateTime FirearmsLicenceReleaseDate { get; set; }
        public DateTime? MedicalExaminationExpireDate { get; set; }
        public string BirthLocation { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CompleteAddress => $"{Address}, {City} ({PostalCode}), {Province} - {Country}";
        public string Phone { get; set; } = string.Empty;
        public string FiscalCode { get; set; } = string.Empty;
        // set when personal data is not provided
        public bool? Warning { get; set; }
        public bool HasWarning => Warning ?? (Warning = this.CalculateWarning(FirearmsLicenceExpireDate, MedicalExaminationExpireDate)).Value;



        public IList<ShooterAssociationContract> Classifications { get; set; } = new List<ShooterAssociationContract>();
        public IList<TeamContract> Teams { get; set; } = new List<TeamContract>();
    }

    public static class ContractExtension
    {
        public static bool CalculateWarning(this ShooterContract _, DateTime firearmsLicenceExpireDate, DateTime? medicalExaminationExpireDate)
        =>
            firearmsLicenceExpireDate.Date <= DateTime.Now ||
                                  !medicalExaminationExpireDate.HasValue || medicalExaminationExpireDate.Value.Date <= DateTime.Now;
    }
}