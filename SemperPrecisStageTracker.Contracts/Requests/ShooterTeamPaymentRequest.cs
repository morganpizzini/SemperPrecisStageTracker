using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class ShooterTeamPaymentRequest{
        [Required]
        public string ShooterTeamPaymentId { get; set; }
    }
}