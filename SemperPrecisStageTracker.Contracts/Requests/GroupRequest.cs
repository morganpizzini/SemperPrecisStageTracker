using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{

    /// <summary>
    /// Group request
    /// </summary>
    public class GroupRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string GroupId { get; set; }
    }
}
