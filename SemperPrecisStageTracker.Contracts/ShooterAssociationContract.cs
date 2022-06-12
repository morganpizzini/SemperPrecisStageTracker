using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterAssociationContract
    {
        public string ShooterAssociationId { get; set; }
        public AssociationContract Association { get; set; }
        public string Classification { get; set; }
        public string Division { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}