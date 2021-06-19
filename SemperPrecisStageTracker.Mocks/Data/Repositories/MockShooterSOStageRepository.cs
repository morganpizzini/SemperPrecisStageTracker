using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockShooterSOStageRepository : MockRepositoryBase<ShooterSOStage, ISemperPrecisStageTrackerScenario>, IShooterSOStageRepository
    {
        public MockShooterSOStageRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterSOStages)
        {
        }
    }
}