using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Group : SemperPrecisEntity
    {
        [Required]
        public string MatchId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime GroupDay { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public int MaxShooterNumber { get; set; }
    }
}