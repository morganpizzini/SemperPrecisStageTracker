using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data.Repositories;

namespace SemperPrecisStageTracker.Domain.Data.Repositories
{

    public interface IFidelityCardTypeRepository : IRepository<FidelityCardType>
    {

    }
    public interface IUserFidelityCardRepository : IRepository<UserFidelityCard>
    {

    }
    public interface IUserFidelityCardAccessRepository : IRepository<UserFidelityCardAccess>
    {

    }
    /// <summary>
    /// Repository interface for "ShooterTeamPayment"
    /// </summary>
    public interface IShooterTeamPaymentRepository : IRepository<TeamPayment>
    {

    }
    /// <summary>
    /// Repository interface for "TeamReminder"
    /// </summary>
    public interface ITeamReminderRepository : IRepository<TeamReminder>
    {

    }

    public interface IBayRepository : IRepository<Bay>
    {

    }

    /// <summary>
    /// Repository interface for "TeamReminder"
    /// </summary>
    public interface IPaymentTypeRepository : IRepository<PaymentType>
    {

    }
    /// <summary>
    /// Repository interface for "Team Holder"
    /// </summary>
    public interface ITeamHolderRepository : IRepository<TeamHolder>
    {
    }
    /// <summary>
    /// Repository interface for "Association"
    /// </summary>
    public interface IAssociationRepository : IRepository<Association>
    {

    }
}