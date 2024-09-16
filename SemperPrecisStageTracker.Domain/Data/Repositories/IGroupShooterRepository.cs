using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{
    /// <summary>
    /// Repository interface for "Group"
    /// </summary>
    public interface IGroupShooterRepository : IRepository<GroupUser>
    {

    }
    public interface IReservationRepository : IRepository<Reservation>
    {

    }
}