using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class ShooterAssociationRepository : EntityFrameworkRepositoryBase<ShooterAssociation, SemperPrecisStageTrackerContext>, IShooterAssociationRepository
    {
        public ShooterAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterAssociations)
        {
        }
    }
    [Repository]
    public class ShooterAssociationInfoRepository : EntityFrameworkRepositoryBase<ShooterAssociationInfo, SemperPrecisStageTrackerContext>, IShooterAssociationInfoRepository
    {
        public ShooterAssociationInfoRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterAssociationInfos)
        {
        }
    }
}