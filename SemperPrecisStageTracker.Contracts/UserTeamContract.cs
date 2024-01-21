using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class UserTeamContract
    {
        public TeamContract Team { get; set; } = new();
        public UserContract User { get; set; } = new();
        public bool UserApprove { get; set; }
        public bool TeamApprove { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}