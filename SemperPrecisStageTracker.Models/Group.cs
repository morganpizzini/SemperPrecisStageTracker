using System;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class BaySchedule : SemperPrecisEntity
    {
        [Required]
        public string ScheduleId { get; set; } = string.Empty;
        [Required]
        public string BayId { get; set; } = string.Empty;
    }

    public class Schedule : SemperPrecisEntity
    {
        [Required]
        public string PlaceId { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public TimeOnly From { get; set; }
        [Required]
        public TimeOnly To { get; set; }
        [Required]
        public DayOfWeek Day { get; set; }

    }
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
        public int MaxUserNumber { get; set; }
    }
}