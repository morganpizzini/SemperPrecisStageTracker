using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockMatchRepository : MockRepositoryBase<Match, ISemperPrecisStageTrackerScenario>, IMatchRepository
    {
        public MockMatchRepository(IDataSession dataSession)
            : base(dataSession, c => c.Matches)
        {
        }
    }

    [Repository]
    public class MockPaymentTypeRepository : MockRepositoryBase<PaymentType, ISemperPrecisStageTrackerScenario>, IPaymentTypeRepository
    {
        public MockPaymentTypeRepository(IDataSession dataSession)
            : base(dataSession, c => c.PaymentTypes)
        {
        }
    }
}