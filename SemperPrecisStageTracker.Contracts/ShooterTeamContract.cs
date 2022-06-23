using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterTeamContract
    {
        public TeamContract Team { get; set; } = new();
        public ShooterContract Shooter { get; set; } = new();
        public bool ShooterApprove { get; set; }
        public bool TeamApprove { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}