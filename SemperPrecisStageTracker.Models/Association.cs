using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{

    public class Association : SemperPrecisEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public IList<string> Divisions { get; set; } = new List<string>();
        [Required]
        public IList<string> Classifications { get; set; } = new List<string>();
        [Required]
        public IList<string> Categories { get; set; } = new List<string>();
    }
}