using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockShooterAssociationRepository : MockRepositoryBase<ShooterAssociation, ISemperPrecisStageTrackerScenario>, IShooterAssociationRepository
    {
        public MockShooterAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterAssociations)
        {
        }
    }

    [Repository]
    public class MockShooterAssociationInfoRepository : MockRepositoryBase<ShooterAssociationInfo, ISemperPrecisStageTrackerScenario>, IShooterAssociationInfoRepository
    {
        public MockShooterAssociationInfoRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterAssociationInfos)
        {
        }
    }
}