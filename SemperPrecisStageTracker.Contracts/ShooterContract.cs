using System;
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
            (new []{FirstName?.ToLower() ?? "", LastName?.ToLower() ?? "", Username?.ToLower() ?? ""})
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();
        
        public DateTime BirthDate { get; set; }
        public string AuthData {get;set;}
        [IndexDbIndex]
        public string Username {get;set;}
        public string Email {get;set;}
        public string FirearmsLicence {get;set;}
        public DateTime FirearmsLicenceExpireDate {get;set;}
        public DateTime MedicalExaminationExpireDate {get;set;}
        public IList<ShooterAssociationContract> Classifications {get;set;} = new List<ShooterAssociationContract>();
        public IList<TeamContract> Teams {get;set;} = new List<TeamContract>();
    }
}