using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Group : SemperPrecisEntity
    {
        [Required]
        public string MatchId {get; set;}
        [Required]
        public string Name { get; set; }
    }
}