using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Utilities
{
    public class NotificationSubscriptionCreateRequest
    {
        [Required]
        public string Url { get; set; }
        [Required]

        public string P256dh { get; set; }
        [Required]

        public string Auth { get; set; }
    }
}
