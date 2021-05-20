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

    public class TeamCreateRequest
    {
        [Required]
        public string Name { get; set; }
    }

    public class TeamUpdateRequest
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string Name { get; set; }
    }

}
