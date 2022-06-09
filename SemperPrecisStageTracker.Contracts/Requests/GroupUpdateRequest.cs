using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class GroupUpdateRequest
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public int MaxShooterNumber { get; set; }

    }
}