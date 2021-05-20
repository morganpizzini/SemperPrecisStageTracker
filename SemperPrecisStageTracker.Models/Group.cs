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

    public class GroupShooter : SemperPrecisEntity
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ShooterId { get; set; }
        [Required]
        public string DivisionId {get; set;}
        [Required]
        public string TeamId {get; set;}
    }
}