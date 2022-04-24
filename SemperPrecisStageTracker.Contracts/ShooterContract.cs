using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterContract
    {
        [IndexDbKey]
        public string ShooterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompleteName => $"{LastName} {FirstName}";
        public DateTime BirthDate { get; set; }
        public string AuthData {get;set;}
        [IndexDbIndex]
        public string Username {get;set;}
        public string Email {get;set;}
        public string FirearmsLicence {get;set;}
        public DateTime FirearmsLicenceExpireDate {get;set;}
        public DateTime MedicalExaminationExpireDate {get;set;}
    }
}