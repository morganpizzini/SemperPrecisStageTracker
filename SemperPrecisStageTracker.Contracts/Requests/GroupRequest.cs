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

    public class GroupCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MatchId { get; set; }
    }

    public class GroupUpdateRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string Name { get; set; }
        
    }

    public class ShooterGroupRequest{
        [Required]

        public string GroupId { get; set; }
        [Required]

        public IList<string> ShooterIds { get; set; }
    }


}
