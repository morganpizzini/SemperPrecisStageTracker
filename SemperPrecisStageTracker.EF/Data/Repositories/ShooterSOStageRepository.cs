using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class ShooterSOStageRepository : EntityFrameworkRepositoryBase<ShooterSOStage, SemperPrecisStageTrackerContext>, IShooterSOStageRepository
    {
        public ShooterSOStageRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterSOStages)
        {
        }
    }
}