using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class AssociationUpdateRequest
    {
        [Required]
        public string AssociationId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<string> Divisions { get; set; } = new List<string>();
        [Required]
        public IList<string> Classifications { get; set; } = new List<string>();
    }
}