using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "AdministrationPermission"
    /// </summary>
    public interface IAdministrationPermissionRepository : IRepository<AdministrationPermission>
    {

    }
    /// <summary>
    /// Repository interface for "EntityPermission"
    /// </summary>
    public interface IEntityPermissionRepository : IRepository<EntityPermission>
    {

    }
    /// <summary>
    /// Repository interface for "Contact"
    /// </summary>
    public interface IContactRepository : IRepository<Contact>
    {

    }
    /// <summary>
    /// Repository interface for "Association"
    /// </summary>
    public interface IAssociationRepository : IRepository<Association>
    {

    }
}