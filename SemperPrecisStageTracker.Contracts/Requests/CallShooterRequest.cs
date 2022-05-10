using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class CallShooterRequest
    {
        [Required]
        public string MatchId { get; set; }

        [Required]
        public string ShooterId { get; set; }

        [Required]
        public CallShooterContextEnum Context { get; set; }
    }
}