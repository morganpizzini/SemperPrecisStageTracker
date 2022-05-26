using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class MatchResultData
    {
        public IList<DivisionMatchResult> Results { get; set; } = new List<DivisionMatchResult>();

        public IList<ShooterClassificationResult> CategoryResults { get; set; } =
            new List<ShooterClassificationResult>();
        public IList<string> StageNames { get; set; } = new List<string>();
    }

    public class DivisionMatchResult
    {
        public string Name { get; set; }
        public IList<ShooterClassificationResult> Classifications { get; set; }
    }
}