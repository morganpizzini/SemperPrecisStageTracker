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
    public class MockAssociationRepository : MockRepositoryBase<Association, ISemperPrecisStageTrackerScenario>, IAssociationRepository
    {
        public MockAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Associations)
        {
        }
    }
        [Repository]
    public class MockTeamRepository : MockRepositoryBase<Team, ISemperPrecisStageTrackerScenario>, ITeamRepository
    {
        public MockTeamRepository(IDataSession dataSession)
            : base(dataSession, c => c.Teams)
        {
        }
    }

            [Repository]
    public class MockNotificationSubscriptionRepository : MockRepositoryBase<NotificationSubscription, ISemperPrecisStageTrackerScenario>, INotificationSubscriptionRepository
    {
        public MockNotificationSubscriptionRepository(IDataSession dataSession)
            : base(dataSession, c => c.NotificationSubscriptions)
        {
        }
    }
}