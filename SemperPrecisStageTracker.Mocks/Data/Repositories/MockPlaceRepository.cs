using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockPlaceRepository : MockRepositoryBase<Place, ISemperPrecisStageTrackerScenario>, IPlaceRepository
    {
        public MockPlaceRepository(IDataSession dataSession)
            : base(dataSession, c => c.Places)
        {
        }
    }

    [Repository]
    public class MockPlaceDataRepository : MockRepositoryBase<PlaceData, ISemperPrecisStageTrackerScenario>, IPlaceDataRepository
    {
        public MockPlaceDataRepository(IDataSession dataSession)
            : base(dataSession, c => c.PlaceDatas)
        {
        }
    }
}