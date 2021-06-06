using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class DivisionMatchResultContract{
        public string Name { get; set; }
        public IList<string> StageNumber {get; set; } = new List<string>();
        public IList<ShooterClassificationResultContract> Classifications { get; set; } = new List<ShooterClassificationResultContract>();
    }
}