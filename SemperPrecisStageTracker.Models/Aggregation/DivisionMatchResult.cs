using System.Collections.Generic;

namespace SemperPrecisStageTracker
{
    public class MatchResultData
    {
        public IList<UserMatchResult> Overall { get; set; } = new List<UserMatchResult>();
        public IList<DivisionMatchResult> Results { get; set; } = new List<DivisionMatchResult>();

        public IList<UserClassificationResult> CategoryResults { get; set; } =
            new List<UserClassificationResult>();
        public IList<string> StageNames { get; set; } = new List<string>();
    }

    public class DivisionMatchResult
    {
        public string Name { get; set; }
        public IList<UserClassificationResult> Classifications { get; set; }
    }
}