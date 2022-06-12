using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class StageUpdateRequest
    {
        [Required]
        public string StageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Index { get; set; }
        public string Scenario { get; set; }
        public string GunReadyCondition { get; set; }
        public string StageProcedure { get; set; }
        public string StageProcedureNotes { get; set; }
        
        ///
        /// Rulebook-2017.-3;
        ///
        public string Rules { get; set; }
        
        public IList<StageStringUpdateRequest> Strings { get; set; } = new List<StageStringUpdateRequest>();
    }

    public class StageStringUpdateRequest
    {
        public string StageStringId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Targets { get; set; }
        
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
        /// From 6 yds to 10 yds
        ///
        public string Distance { get; set; }
        ///
        /// Required
        ///
        public bool CoverGarment { get; set; }
    }
}