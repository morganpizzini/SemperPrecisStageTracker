using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupShooterDeleteRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }
}