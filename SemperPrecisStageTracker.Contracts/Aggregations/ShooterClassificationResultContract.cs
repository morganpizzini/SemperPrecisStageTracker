using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class ShooterClassificationResultContract{
        public string Classification { get; set; }
        public IList<ShooterMatchResultContract> Shooters { get; set; } = new List<ShooterMatchResultContract>();

    }
}