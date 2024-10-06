using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    
    [Repository]
    public class PermissionsRoleRepository : EntityFrameworkRepositoryBase<PermissionRole, SemperPrecisStageTrackerContext>, IPermissionsRoleRepository
    {
        public PermissionsRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionsRoles)
        {
        }
    }

    [Repository]
    public class RoleRepository : EntityFrameworkRepositoryBase<Role, SemperPrecisStageTrackerContext>, IRoleRepository
    {
        public RoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.Roles)
        {
        }
    }

    [Repository]
    public class UserRoleRepository : EntityFrameworkRepositoryBase<UserRole, SemperPrecisStageTrackerContext>, IUserRoleRepository
    {
        public UserRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserRoles)
        {
        }
    }

    [Repository]
    public class UserPermissionRepository : EntityFrameworkRepositoryBase<UserPermission, SemperPrecisStageTrackerContext>, IUserPermissionRepository
    {
        public UserPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserPermissions)
        {
        }
    }

    [Repository]
    public class PermissionGroupRepository : EntityFrameworkRepositoryBase<PermissionGroup, SemperPrecisStageTrackerContext>, IPermissionGroupRepository
    {
        public PermissionGroupRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionGroups)
        {
        }
    }

    [Repository]
    public class UserPermissionGroupRepository : EntityFrameworkRepositoryBase<UserPermissionGroup, SemperPrecisStageTrackerContext>, IUserPermissionGroupRepository
    {
        public UserPermissionGroupRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserPermissionGroups)
        {
        }
    }

    [Repository]
    public class PermissionGroupRoleRepository : EntityFrameworkRepositoryBase<PermissionGroupRole, SemperPrecisStageTrackerContext>, IPermissionGroupRoleRepository
    {
        public PermissionGroupRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionGroupRoles)
        {
        }
    }
}