﻿using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "Shooter"
    /// </summary>
    public interface IShooterRepository : IRepository<Shooter>
    {

    }
    /// <summary>
    /// Repository interface for "ShooterData"
    /// </summary>
    public interface IShooterDataRepository : IRepository<ShooterData>
    {

    }
}