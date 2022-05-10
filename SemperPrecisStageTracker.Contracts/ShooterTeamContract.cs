using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterTeamContract
    {
        public TeamContract Team { get; set; }
        public ShooterContract Shooter { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}