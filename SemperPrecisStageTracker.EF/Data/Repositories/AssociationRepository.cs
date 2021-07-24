using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class EntityPermissionRepository : EntityFrameworkRepositoryBase<EntityPermission, SemperPrecisStageTrackerContext>, IEntityPermissionRepository
    {
        public EntityPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.EntityPermissions)
        {
        }
    }
    [Repository]
    public class AdministrationPermissionRepository : EntityFrameworkRepositoryBase<AdministrationPermission, SemperPrecisStageTrackerContext>, IAdministrationPermissionRepository
    {
        public AdministrationPermissionRepository(IDataSession dataSession)
            : base(dataSession, c => c.AdministrationPermissions)
        {
        }
    }
    [Repository]
    public class AssociationRepository : EntityFrameworkRepositoryBase<Association, SemperPrecisStageTrackerContext>, IAssociationRepository
    {
        public AssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Associations)
        {
        }
    }

    [Repository]
    public class ContactRepository : EntityFrameworkRepositoryBase<Contact, SemperPrecisStageTrackerContext>, IContactRepository
    {
        public ContactRepository(IDataSession dataSession)
            : base(dataSession, c => c.Contacts)
        {
        }
    }
}