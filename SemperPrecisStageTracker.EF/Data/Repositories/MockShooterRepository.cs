﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class MockShooterRepository : EntityFrameworkRepositoryBase<Shooter, SemperPrecisStageTrackerContext>, IShooterRepository
    {
        public MockShooterRepository(IDataSession dataSession)
            : base(dataSession, c => c.Shooters)
        {
        }
    }

    [Repository]
    public class MockShooterTeamRepository : EntityFrameworkRepositoryBase<ShooterTeam, SemperPrecisStageTrackerContext>, IShooterTeamRepository
    {
        public MockShooterTeamRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterTeams)
        {
        }
    }

        [Repository]
    public class MockShooterAssociationRepository : EntityFrameworkRepositoryBase<ShooterAssociation, SemperPrecisStageTrackerContext>, IShooterAssociationRepository
    {
        public MockShooterAssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterAssociations)
        {
        }
    }
}