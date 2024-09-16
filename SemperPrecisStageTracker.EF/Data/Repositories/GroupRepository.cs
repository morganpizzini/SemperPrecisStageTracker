using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class GroupRepository : EntityFrameworkRepositoryBase<Group, SemperPrecisStageTrackerContext>, IGroupRepository
    {
        public GroupRepository(IDataSession dataSession)
            : base(dataSession, c => c.Groups)
        {
        }
    }

    [Repository]
    public class ReservationRepository : EntityFrameworkRepositoryBase<Reservation, SemperPrecisStageTrackerContext>, IReservationRepository
    {
        public ReservationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Reservations)
        {
        }
    }
}
