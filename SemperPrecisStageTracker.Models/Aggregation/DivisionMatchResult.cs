using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class DivisionMatchResult
    {
        public string Name { get; set; }
        public IList<string> StageNumber { get; set; } = new List<string>();
        public IList<ShooterClassificationResult> Classifications { get; set; }
    }
}