using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockPermissionRepository : MockRepositoryBase<Permission, ISemperPrecisStageTrackerScenario>, IPermissionRepository
    {
        public MockPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.Permissions)
        {
        }
    }

    
[Repository]
    public class MockPermissionsRoleRepository : MockRepositoryBase<PermissionsRole, ISemperPrecisStageTrackerScenario>, IPermissionsRoleRepository
    {
        public MockPermissionsRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionsRoles)
        {
        }
    }

[Repository]
    public class MockRoleRepository : MockRepositoryBase<Role, ISemperPrecisStageTrackerScenario>, IRoleRepository
    {
        public MockRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.Roles)
        {
        }
    }

[Repository]
    public class MockUserRoleRepository : MockRepositoryBase<UserRole, ISemperPrecisStageTrackerScenario>, IUserRoleRepository
    {
        public MockUserRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserRoles)
        {
        }
    }

[Repository]
    public class MockUserPermissionRepository : MockRepositoryBase<UserPermission, ISemperPrecisStageTrackerScenario>, IUserPermissionRepository
    {
        public MockUserPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserPermissions)
        {
        }
    }

[Repository]
    public class MockPermissionGroupRepository : MockRepositoryBase<PermissionGroup, ISemperPrecisStageTrackerScenario>, IPermissionGroupRepository
    {
        public MockPermissionGroupRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionGroups)
        {
        }
    }

[Repository]
    public class MockUserPermissionGroupRepository : MockRepositoryBase<UserPermissionGroup, ISemperPrecisStageTrackerScenario>, IUserPermissionGroupRepository
    {
        public MockUserPermissionGroupRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserPermissionGroups)
        {
        }
    }

[Repository]
    public class MockPermissionGroupRoleRepository : MockRepositoryBase<PermissionGroupRole, ISemperPrecisStageTrackerScenario>, IPermissionGroupRoleRepository
    {
        public MockPermissionGroupRoleRepository(IDataSession dataSession)
            : base(dataSession, c => c.PermissionGroupRoles)
        {
        }
    }
}