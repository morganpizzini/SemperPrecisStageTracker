using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockAdministrationPermissionRepository : MockRepositoryBase<AdministrationPermission, ISemperPrecisStageTrackerScenario>, IAdministrationPermissionRepository
    {
        public MockAdministrationPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.AdministrationPermissions)
        {
        }
    }
    [Repository]
    public class MockEntityPermissionRepository : MockRepositoryBase<EntityPermission, ISemperPrecisStageTrackerScenario>, IEntityPermissionRepository
    {
        public MockEntityPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.EntityPermissions)
        {
        }
    }
    [Repository]
    public class MockContactRepository : MockRepositoryBase<Contact, ISemperPrecisStageTrackerScenario>, IContactRepository
    {
        public MockContactRepository(IDataSession dataSession)
            : base(dataSession, c => c.Contacts)
        {
        }
    }
    [Repository]
    public class MockAssociationRepository : MockRepositoryBase<Association, ISemperPrecisStageTrackerScenario>, IAssociationRepository
    {
        public MockAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Associations)
        {
        }
    }
}