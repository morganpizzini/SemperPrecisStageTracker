using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class DivisionMatchResultContract
    {
        public string Name { get; set; }
        public IList<ShooterClassificationResultContract> Classifications { get; set; } = new List<ShooterClassificationResultContract>();
    }
}