using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class ShooterClassificationResult
    {
        public string Classification { get; set; }
        public IList<ShooterMatchResult> Shooters { get; set; }

    }
}