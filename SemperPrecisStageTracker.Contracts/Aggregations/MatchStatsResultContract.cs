using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchStatsResultContract{
        public MatchContract Match { get; set; }
        public IList<DivisionMatchResultContract> DivisionMatchResults { get; set; } = new List<DivisionMatchResultContract>();
    }
}