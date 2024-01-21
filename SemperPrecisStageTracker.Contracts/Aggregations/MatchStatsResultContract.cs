using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchStatsResultContract
    {
        public MatchContract Match { get; set; }
        public IList<string> StageNames { get; set; } = new List<string>();
        public IList<UserMatchResultContract> Overall { get; set; } = new List<UserMatchResultContract>();

        public IList<DivisionMatchResultContract> DivisionMatchResults { get; set; } = new List<DivisionMatchResultContract>();
        public IList<UserClassificationResultContract> CategoryResults { get; set; } = new List<UserClassificationResultContract>();
    }
}