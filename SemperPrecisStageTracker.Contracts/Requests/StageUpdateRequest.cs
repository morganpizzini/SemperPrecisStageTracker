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
        public string Description { get; set; }
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
}