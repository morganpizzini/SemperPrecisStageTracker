using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class NotificationSubscription : SemperPrecisEntity
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string P256dh { get; set; }
        [Required]
        public string Auth { get; set; }
    }
}