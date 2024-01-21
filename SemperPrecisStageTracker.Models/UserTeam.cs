using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class UserTeam : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool UserApprove { get; set; }
        [Required]
        public bool TeamApprove { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}