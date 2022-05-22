using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class AssociationCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions { get; set; } = new List<string>();
        [Required]
        public IList<string> Classifications { get; set; } = new List<string>();
        [Required]
        public IList<string> Categories { get; set; } = new List<string>();
    }
}