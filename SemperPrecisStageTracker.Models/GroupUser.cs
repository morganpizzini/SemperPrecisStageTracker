using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class GroupUser : SemperPrecisEntity
    {
        public string MatchId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string DivisionId { get; set; } = string.Empty;
        //[Required]
        public string Classification { get; set; } = string.Empty;
        [Required]
        public string TeamId { get; set; } = string.Empty;
        
        public DateTime? HasPay { get; set; }
    }
}