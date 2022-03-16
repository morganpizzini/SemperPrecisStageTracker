using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.EF.Context;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Data.Repositories.Attributes;
using ZenProgramming.Chakra.Core.EntityFramework.Data.Repositories;

namespace SemperPrecisStageTracker.EF.Data.Repositories
{
    [Repository]
    public class ShooterTeamPaymentRepository : EntityFrameworkRepositoryBase<ShooterTeamPayment, SemperPrecisStageTrackerContext>, IShooterTeamPaymentRepository
    {
        public ShooterTeamPaymentRepository(IDataSession dataSession)
            : base(dataSession, c => c.ShooterTeamPayments)
        {
        }
    }
    [Repository]
    public class TeamHolderRepository : EntityFrameworkRepositoryBase<TeamHolder, SemperPrecisStageTrackerContext>, ITeamHolderRepository
    {
        public TeamHolderRepository(IDataSession dataSession)
            : base(dataSession, c => c.TeamHolders)
        {
        }
    }
    [Repository]
    public class AssociationRepository : EntityFrameworkRepositoryBase<Association, SemperPrecisStageTrackerContext>, IAssociationRepository
    {
        public AssociationRepository(IDataSession dataSession)
            : base(dataSession, c => c.Associations)
        {
        }
    }
}