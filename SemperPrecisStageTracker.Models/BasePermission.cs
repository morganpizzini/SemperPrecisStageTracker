using SemperPrecisStageTracker.Models.Commons;

namespace SemperPrecisStageTracker.Models
{
    public abstract class BasePermission : SemperPrecisEntity
    {
        public string ShooterId { get; set; }
        public string Permission { get; set; }
    }

    public class AdministrationPermission : BasePermission
    {
    }

    public class EntityPermission : BasePermission
    {
        public string EntityId { get; set; }
    }
}