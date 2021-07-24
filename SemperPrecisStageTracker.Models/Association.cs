using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
   
    public class Association : SemperPrecisEntity {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions {get;set;}
        [Required]
        public IList<string> Classifications {get;set;}
    }
}