using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MatchId { get; set; }
    }
}