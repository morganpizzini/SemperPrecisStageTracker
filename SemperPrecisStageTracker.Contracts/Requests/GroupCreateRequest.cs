using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MatchId { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime GroupDay { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public int MaxShooterNumber { get; set; }
    }
}