using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{

    /// <summary>
    /// Stage request
    /// </summary>
    public class StageRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string StageId { get; set; }

    }
}
