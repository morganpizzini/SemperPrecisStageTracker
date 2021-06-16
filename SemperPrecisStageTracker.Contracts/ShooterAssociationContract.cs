using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterAssociationContract {
        public AssociationContract Association{get;set;}
        public string Classification { get; set; }
        public string CardNumber { get; set; }
        public bool SafetyOfficier {get; set;}
        public DateTime RegistrationDate { get; set; }
    }
}