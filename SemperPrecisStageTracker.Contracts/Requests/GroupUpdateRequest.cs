using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupUpdateRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}