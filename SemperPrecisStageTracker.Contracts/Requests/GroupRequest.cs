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

    /// <summary>
    /// Group stage request
    /// </summary>
    public class GroupStageRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string StageId { get; set; }
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
    public class GroupShooterCreateRequest
    {
        [Required]

        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string DivisionId { get; set; }
        [Required]
        public string TeamId { get; set; }
    }
    public class GroupShooterDeleteRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }


}
