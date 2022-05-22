using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class MatchResultData
    {
        public IList<DivisionMatchResult> Results { get; set; } = new List<DivisionMatchResult>();

        public IList<ShooterClassificationResult> CategoryResults { get; set; } =
            new List<ShooterClassificationResult>();
    }

    public class DivisionMatchResult
    {
        public string Name { get; set; }
        public IList<string> StageNumber { get; set; } = new List<string>();
        public IList<ShooterClassificationResult> Classifications { get; set; }
    }
}