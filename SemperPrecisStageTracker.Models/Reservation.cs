using SemperPrecisStageTracker.Models.Commons;
using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Models
{
    public class Reservation : SemperPrecisEntity
    {
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string BayId { get; set; } = string.Empty;
        [Required]
        public TimeOnly From { get; set; }
        [Required]
        public TimeOnly To { get; set; }
        [Required]
        public DateOnly Day { get; set; }
        public string Demands { get; set; } = string.Empty;
        public bool IsAccepted { get; set; } = false;
        // whenever is true, the bay is blocked for the time of the reservation, and cannot be reserved by anyone else
        public bool IsBayBlocked { get; set; } = false;
    }
}