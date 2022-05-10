using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
