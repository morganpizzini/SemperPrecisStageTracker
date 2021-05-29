using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class MockMatchRepository : EntityFrameworkRepositoryBase<Match, SemperPrecisStageTrackerContext>, IMatchRepository
    {
        public MockMatchRepository(IDataSession dataSession)
            : base(dataSession, c => c.Matches)
        {
        }
    }
    [Repository]
    public class MockAssociationRepository : EntityFrameworkRepositoryBase<Association, SemperPrecisStageTrackerContext>, IAssociationRepository
    {
        public MockAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Associations)
        {
        }
    }
        [Repository]
    public class MockTeamRepository : EntityFrameworkRepositoryBase<Team, SemperPrecisStageTrackerContext>, ITeamRepository
    {
        public MockTeamRepository(IDataSession dataSession)
            : base(dataSession, c => c.Teams)
        {
        }
    }

    [Repository]
    public class NotificationSubscriptionRepository : EntityFrameworkRepositoryBase<NotificationSubscription, SemperPrecisStageTrackerContext>, INotificationSubscriptionRepository
    {
        public NotificationSubscriptionRepository(IDataSession dataSession)
            : base(dataSession, c => c.NotificationSubscriptions)
        {
        }
    }
}