﻿using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "Stage"
    /// </summary>
    public interface IStageRepository : IRepository<Stage>
    {

    }
    /// <summary>
    /// Repository interface for "StageString"
    /// </summary>
    public interface IStageStringRepository : IRepository<StageString>
    {

    }
}