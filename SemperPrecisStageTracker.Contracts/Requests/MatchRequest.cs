using System;
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

    public class MatchCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime MatchDateTime { get; set; }
    }

    public class MatchUpdateRequest
    {
        [Required]
        public string MatchId { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public DateTime MatchDateTime { get; set; }
    }

}
