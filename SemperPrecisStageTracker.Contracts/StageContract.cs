using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class StageContract
    {
        [IndexDbKey]
        public string StageId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public MatchContract Match { get; set; }
        public string Scenario { get; set; }
        public string GunReadyCondition { get; set; }
        public string StageProcedure { get; set; }
        public string StageProcedureNotes { get; set; }
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules { get; set; }

        public IList<StageStringContract> Strings { get; set; } = new List<StageStringContract>();
    }
}