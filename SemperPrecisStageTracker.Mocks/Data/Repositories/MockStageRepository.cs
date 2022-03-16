﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockShooterTeamPaymentRepository : MockRepositoryBase<ShooterTeamPayment, ISemperPrecisStageTrackerScenario>, IShooterTeamPaymentRepository
    {
        public MockShooterTeamPaymentRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterTeamPayments)
        {
        }
    }
    [Repository]
    public class MockTeamHolderRepository : MockRepositoryBase<TeamHolder, ISemperPrecisStageTrackerScenario>, ITeamHolderRepository
    {
        public MockTeamHolderRepository(IDataSession dataSession)
            : base(dataSession, c => c.TeamHolders)
        {
        }
    }
    [Repository]
    public class MockStageRepository : MockRepositoryBase<Stage, ISemperPrecisStageTrackerScenario>, IStageRepository
    {
        public MockStageRepository(IDataSession dataSession)
            : base(dataSession, c => c.Stages)
        {
        }
    }
}