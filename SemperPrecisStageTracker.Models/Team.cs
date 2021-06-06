using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public class Team : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; }
    }
}