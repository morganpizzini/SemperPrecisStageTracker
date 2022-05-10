namespace SemperPrecisStageTracker.Contracts
{
    public class StageContract
    {
        [IndexDbKey]
        public string StageId { get; set; }
        [IndexDbIndex]
        public string Name { get; set; }
        public int Targets { get; set; }
        public int Index { get; set; }
        public MatchContract Match { get; set; }
        public string SO { get; set; }
        public string Scenario { get; set; }
        public string GunReadyCondition { get; set; }
        public string StageProcedure { get; set; }
        public string StageProcedureNotes { get; set; }
        public int Strings { get; set; }

        ///
        /// 12 rounds min, Unlimited
        ///
        public string Scoring { get; set; }
        ///
        /// 4 threat, 2 non threat.
        ///
        public string TargetsDescription { get; set; }
        ///
        /// Best 2 per paper
        ///
        public string ScoredHits { get; set; }
        ///
        /// Audible - Last shot
        ///
        public string StartStop { get; set; }
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules { get; set; }
        ///
        /// From 6 yds to 10 yds
        ///
        public string Distance { get; set; }
        ///
        /// Required
        ///
        public bool CoverGarment { get; set; }
    }
}