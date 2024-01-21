using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public abstract class UserRelationRole : SemperPrecisEntity
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}