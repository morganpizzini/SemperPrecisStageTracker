﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class ShooterRepository : EntityFrameworkRepositoryBase<Shooter, SemperPrecisStageTrackerContext>, IShooterRepository
    {
        public ShooterRepository(IDataSession dataSession)
            : base(dataSession, c => c.Shooters)
        {
        }
    }

    [Repository]
    public class ShooterDataRepository : EntityFrameworkRepositoryBase<ShooterData, SemperPrecisStageTrackerContext>, IShooterDataRepository
    {
        public ShooterDataRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterDatas)
        {
        }
    }
}