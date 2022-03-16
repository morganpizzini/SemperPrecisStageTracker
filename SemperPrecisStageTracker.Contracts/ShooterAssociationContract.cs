using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class TeamHolderContract{
        public string  TeamHolderId {get;set;}
        public TeamContract Team{get;set;}
        public ShooterContract Shooter{get;set;}
        public string Description { get; set; }
        
    }
    public class ShooterAssociationContract {
        public string  ShooterAssociationId {get;set;}
        public AssociationContract Association{get;set;}
        public string Classification { get; set; }
        public string Division { get; set; }
        public string CardNumber { get; set; }
        public bool SafetyOfficier {get; set;}
        public DateTime RegistrationDate { get; set; }
    }
}