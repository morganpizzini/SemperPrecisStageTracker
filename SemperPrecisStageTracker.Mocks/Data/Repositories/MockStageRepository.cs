﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockStageRepository : MockRepositoryBase<Stage, ISemperPrecisStageTrackerScenario>, IStageRepository
    {
        public MockStageRepository(IDataSession dataSession)
            : base(dataSession, c => c.Stages)
        {
        }
    
        [Repository]
        public class MockStageStringRepository : MockRepositoryBase<StageString, ISemperPrecisStageTrackerScenario>, IStageStringRepository 
        {
            public MockStageStringRepository (IDataSession dataSession)
                : base(dataSession, c => c.StageStrings)
            {
            }
        }
}