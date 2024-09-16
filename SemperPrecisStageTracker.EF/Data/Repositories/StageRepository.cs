using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
     [Repository]
    public class TeamReminderRepository : EntityFrameworkRepositoryBase<TeamReminder, SemperPrecisStageTrackerContext>, ITeamReminderRepository
    {
        public TeamReminderRepository(IDataSession dataSession)
            : base(dataSession, c => c.TeamReminder)
        {
        }
    }

    [Repository]
    public class BayRepository : EntityFrameworkRepositoryBase<Bay, SemperPrecisStageTrackerContext>, IBayRepository
    {
        public BayRepository(IDataSession dataSession)
            : base(dataSession, c => c.Bays)
        {
        }
    }
    [Repository]
    public class StageRepository : EntityFrameworkRepositoryBase<Stage, SemperPrecisStageTrackerContext>, IStageRepository
    {
        public StageRepository(IDataSession dataSession)
            : base(dataSession, c => c.Stages)
        {
        }
    }

    [Repository]
    public class StageStringRepository : EntityFrameworkRepositoryBase<StageString, SemperPrecisStageTrackerContext>, IStageStringRepository
    {
        public StageStringRepository (IDataSession dataSession)
            : base(dataSession, c => c.StageStrings)
        {
        }
    }
}