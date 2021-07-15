﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.Mocks.Data.Repositories;

namespace SemperPrecisStageTracker.Mocks.Data.Repositories
{
    [Repository]
    public class MockContactRepository : MockRepositoryBase<Contact, ISemperPrecisStageTrackerScenario>, IContactRepository
    {
        public MockContactRepository(IDataSession dataSession)
            : base(dataSession, c => c.Contacts)
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
}