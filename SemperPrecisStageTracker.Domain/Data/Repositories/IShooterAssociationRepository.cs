using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "ShooterAssociation"
    /// </summary>
    public interface IShooterAssociationRepository : IRepository<UserAssociation>
    {

    }

    /// <summary>
    /// Repository interface for "ShooterAssociationInfo"
    /// </summary>
    public interface IShooterAssociationInfoRepository : IRepository<UserAssociationInfo>
    {

    }
}