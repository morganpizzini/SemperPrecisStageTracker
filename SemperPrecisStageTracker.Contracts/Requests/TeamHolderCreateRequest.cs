using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class TeamHolderCreateRequest
    {
        [Required]

        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}