using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Match request
    /// </summary>
    public class MatchRequest : EntityFilterValidation
    {
        public override string EntityId => MatchId;
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string MatchId { get; set; }

    }
    /// <summary>
    /// Match request
    /// </summary>
    public class MatchCompetitionReadyRequest : EntityFilterValidation
    {
        public override string EntityId => MatchId;
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string MatchId { get; set; }

        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public bool Ready { get; set; }

    }
}
