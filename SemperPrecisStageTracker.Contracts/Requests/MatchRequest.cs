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
        public DateTime MatchDateTime { get; set; } = DateTime.Now;
        [Required]
        public string AssociationId { get; set; }
        public string Location {get;set;}
        public bool UnifyRanks { get; set; }
        public bool OpenMatch {get; set; }

    }

    public class MatchUpdateRequest
    {
        [Required]
        public string MatchId { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public DateTime MatchDateTime { get; set; }
        [Required]
        public string AssociationId { get; set; }
        public string Location {get;set;}
        public bool UnifyRanks { get; set; }
        public bool OpenMatch {get; set; }

    }

    public class SignInRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

        /// <summary>
    /// Request for change password
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// old password
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// new password
        /// </summary>
        public string Password { get; set; }
    }

}
