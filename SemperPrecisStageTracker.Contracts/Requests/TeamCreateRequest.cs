using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class TeamCreateRequest
    {
        [Required]
        public string Name { get; set; }
    }
}