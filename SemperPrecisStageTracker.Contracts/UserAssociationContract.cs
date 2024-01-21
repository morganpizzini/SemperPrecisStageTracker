using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserAssociationContract
    {
        public string UserAssociationId { get; set; }
        public AssociationContract Association { get; set; }
        public string Classification { get; set; }
        public string Division { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}