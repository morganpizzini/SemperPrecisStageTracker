using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class TeamHolderDeleteRequest
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }
    public class ShooterTeamDeleteRequest
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }
}