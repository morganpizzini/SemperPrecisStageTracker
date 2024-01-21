﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class GroupShooterRepository : EntityFrameworkRepositoryBase<GroupUser, SemperPrecisStageTrackerContext>, IGroupShooterRepository
    {
        public GroupShooterRepository(IDataSession dataSession)
            : base(dataSession, c => c.GroupShooters)
        {
        }
    }

    [Repository]
    public class PaymentTypeRepository : EntityFrameworkRepositoryBase<PaymentType, SemperPrecisStageTrackerContext>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(IDataSession dataSession)
            : base(dataSession, c => c.PaymentTypes)
        {
        }
    }
}