using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "Permission"
    /// </summary>
    public interface IPermissionRepository : IRepository<Permission>
    {
    }
    /// <summary>
    /// Repository interface for "PermissionRole"
    /// </summary>
    public interface IPermissionsRoleRepository : IRepository<PermissionRole>
    {
    }

    /// <summary>
    /// Repository interface for "Role"
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
    }

    /// <summary>
    /// Repository interface for "UserRole"
    /// </summary>
    public interface IUserRoleRepository : IRepository<UserRole>
    {
    }

    /// <summary>
    /// Repository interface for "UserPermission"
    /// </summary>
    public interface IUserPermissionRepository : IRepository<UserPermission>
    {
    }

    /// <summary>
    /// Repository interface for "PermissionGroup"
    /// </summary>
    public interface IPermissionGroupRepository : IRepository<PermissionGroup>
    {
    }

    /// <summary>
    /// Repository interface for "UserPermissionGroup"
    /// </summary>
    public interface IUserPermissionGroupRepository : IRepository<UserPermissionGroup>
    {
    }

    /// <summary>
    /// Repository interface for "PermissionGroupRole"
    /// </summary>
    public interface IPermissionGroupRoleRepository : IRepository<PermissionGroupRole>
    {
    }

}