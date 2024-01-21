using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Blazor.Models
{
    public class ClientSetting
    {
        public bool OfflineMode { get; set; }
        [Required]
        public string MatchId { get; set; } = string.Empty;
    }
}