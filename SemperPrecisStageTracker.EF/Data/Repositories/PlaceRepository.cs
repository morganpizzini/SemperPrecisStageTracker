﻿using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class PlaceRepository : EntityFrameworkRepositoryBase<Place, SemperPrecisStageTrackerContext>, IPlaceRepository
    {
        public PlaceRepository(IDataSession dataSession)
            : base(dataSession, c => c.Places)
        {
        }
    }
    [Repository]
    public class PlaceDataRepository : EntityFrameworkRepositoryBase<PlaceData, SemperPrecisStageTrackerContext>, IPlaceDataRepository
    {
        public PlaceDataRepository(IDataSession dataSession)
            : base(dataSession, c => c.PlaceDatas)
        {
        }
    }
}