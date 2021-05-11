using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Match : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
    }
}