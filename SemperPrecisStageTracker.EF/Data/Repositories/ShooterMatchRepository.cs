﻿
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class ShooterMatchRepository : EntityFrameworkRepositoryBase<ShooterMatch, SemperPrecisStageTrackerContext>, IShooterMatchRepository
    {
        public ShooterMatchRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterMatches)
        {
        }
    }
}