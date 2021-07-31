using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public abstract class BasePermission : SemperPrecisEntity
    {
        public string ShooterId { get; set; }
        public string Permission { get; set; }
    }
}