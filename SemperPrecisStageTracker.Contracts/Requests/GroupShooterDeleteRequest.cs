using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupShooterRequest
    {
        [Required]
        public string GroupShooterId { get; set; }
    }

    public class GroupShooterDeleteRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
    }
}