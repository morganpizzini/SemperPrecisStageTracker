using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    /// <summary>
    /// Team request
    /// </summary>
    public class TeamRequest: EntityFilterValidation
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string TeamId { get; set; }

        public override string EntityId => TeamId;

    }
}
