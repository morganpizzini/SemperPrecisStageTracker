using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Stage : SemperPrecisEntity
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        ///
        /// Stage index for sorting
        ///
        public int Index { get; set; }
        public int Targets { get; set; }
        
    }
}