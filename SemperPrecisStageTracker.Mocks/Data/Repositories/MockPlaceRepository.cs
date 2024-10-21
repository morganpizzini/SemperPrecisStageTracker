using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class FidelityCardTypeRepository : MockRepositoryBase<FidelityCardType, ISemperPrecisStageTrackerScenario>, IFidelityCardTypeRepository
    {
        public FidelityCardTypeRepository(IDataSession dataSession)
            : base(dataSession, c => c.FidelityCardTypes)
        {
        }
    }
    [Repository]
    public class UserFidelityCardRepository : MockRepositoryBase<UserFidelityCard, ISemperPrecisStageTrackerScenario>, IUserFidelityCardRepository
    {
        public UserFidelityCardRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserFidelityCards)
        {
        }
    }
    [Repository]
    public class UserFidelityCardAccessRepository : MockRepositoryBase<UserFidelityCardAccess, ISemperPrecisStageTrackerScenario>, IUserFidelityCardAccessRepository
    {
        public UserFidelityCardAccessRepository(IDataSession dataSession)
            : base(dataSession, c => c.UserFidelityCardAccesses)
        {
        }
    }
    [Repository]
    public class MockPlaceRepository : MockRepositoryBase<Place, ISemperPrecisStageTrackerScenario>, IPlaceRepository
    {
        public MockPlaceRepository(IDataSession dataSession)
            : base(dataSession, c => c.Places)
        {
        }
    }
}