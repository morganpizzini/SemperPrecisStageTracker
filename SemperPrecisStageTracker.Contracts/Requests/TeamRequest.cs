using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Team request
    /// </summary>
    public class TeamRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string TeamId { get; set; }
       
    }
}
