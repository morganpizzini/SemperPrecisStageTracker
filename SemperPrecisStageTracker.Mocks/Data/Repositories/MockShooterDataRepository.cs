﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories;

[Repository]
public class MockShooterDataRepository : MockRepositoryBase<ShooterData, ISemperPrecisStageTrackerScenario>, IShooterDataRepository
{
    public MockShooterDataRepository(IDataSession dataSession)
        : base(dataSession, c => c.ShooterDatas)
    {
    }
}