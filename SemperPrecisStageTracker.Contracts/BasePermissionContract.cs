namespace SemperPrecisStageTracker.Contracts
{
    public abstract class BasePermissionContract
    {
        public int Permission { get; set; }
    }
    public class AdministrationPermissionContract : BasePermissionContract
    {
        public string AdministrationPermissionId { get; set; }
    }

    public class EntityPermissionContract : BasePermissionContract
    {
        public string EntityPermissionId { get; set; }
        public string EntityId { get; set; }
    }
}