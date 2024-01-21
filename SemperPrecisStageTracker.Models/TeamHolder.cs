using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class TeamHolder : SemperPrecisEntity
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}