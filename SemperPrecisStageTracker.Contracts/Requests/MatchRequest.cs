using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Match request
    /// </summary>
    public class MatchRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string MatchId { get; set; }

    }
}
