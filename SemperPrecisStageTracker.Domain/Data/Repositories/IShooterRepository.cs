using SemperPrecisStageTracker.Models;
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
    /// Repository interface for "ShooterTeam"
    /// </summary>
    public interface IShooterTeamRepository : IRepository<ShooterTeam>
    {

    }

        /// <summary>
    /// Repository interface for "ShooterAssociation"
    /// </summary>
    public interface IShooterAssociationRepository : IRepository<ShooterAssociation>
    {

    }
}