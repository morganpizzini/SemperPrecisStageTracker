using System.Collections.Generic;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker.Mocks.Scenarios.Common
{
    /// <summary>
    /// Base classe for application scenario
    /// </summary>
    public abstract class SemperPrecisStageTrackerScenarioBase : ISemperPrecisStageTrackerScenario
    {
        /// <summary>
        /// List of groups
        /// </summary>
        public IList<Group> Groups { get; set; } = new List<Group>();

        /// <summary>
        /// List of group shooters
        /// </summary>
        public IList<GroupShooter> GroupShooters { get; set; } = new List<GroupShooter>();

        /// <summary>
        /// List of team shooters
        /// </summary>
        public IList<ShooterTeam> ShooterTeams { get; set; } = new List<ShooterTeam>();

        /// <summary>
        /// List of match
        /// </summary>
        public IList<Match> Matches { get; set; } = new List<Match>();

        /// <summary>
        /// List of associations
        /// </summary>
        public IList<Association> Associations { get; set; } = new List<Association>();

        /// <summary>
        /// List of shooter
        /// </summary>
        public IList<Shooter> Shooters { get; set; } = new List<Shooter>();

        /// <summary>
        /// List of stage
        /// </summary>
        public IList<Stage> Stages { get; set; } = new List<Stage>();

        /// <summary>
        /// List of shooter stage
        /// </summary>
        public IList<ShooterStage> ShooterStages { get; set; } = new List<ShooterStage>();

        /// <summary>
        /// List of group shooters
        /// </summary>
        public IList<ShooterMatch> ShooterMatches { get; set; } = new List<ShooterMatch>();

        /// <summary>
        /// List of group shooters
        /// </summary>
        public IList<ShooterSOStage> ShooterSOStages { get; set; } = new List<ShooterSOStage>();

        /// <summary>
        /// List of teams
        /// </summary>
        public IList<Team> Teams { get; set; } = new List<Team>();

        /// <summary>
        /// List of teams
        /// </summary>
        public IList<ShooterAssociation> ShooterAssociations { get; set; } = new List<ShooterAssociation>();

        /// <summary>
        /// List of notification subscription
        /// </summary>
        public IList<NotificationSubscription> NotificationSubscriptions { get; set; } = new List<NotificationSubscription>();

        /// <summary>
        /// List of places
        /// </summary>
        public IList<Place> Places { get; set; } = new List<Place>();

        /// <summary>
        /// List of contacts
        /// </summary>
        public IList<Contact> Contacts { get; set; } = new List<Contact>();

        /// <summary>
        /// List of administration permissions
        /// </summary>
        public IList<AdministrationPermission> AdministrationPermissions { get; set; } = new List<AdministrationPermission>();

        /// <summary>
        /// List of entity permissions
        /// </summary>
        public IList<EntityPermission> EntityPermissions { get; set; } = new List<EntityPermission>();

        /// <summary>
        /// List of team holders
        /// </summary>
        public IList<TeamHolder> TeamHolders { get; set; } = new List<TeamHolder>();

        /// <summary>
        /// List of Shooter Team Payments
        /// </summary>
        public IList<ShooterTeamPayment> ShooterTeamPayments { get; set; } = new List<ShooterTeamPayment>();

        /// <summary>
        /// Executes initialization of entities
        /// </summary>
        public abstract void InitializeEntities();

        /// <summary>
        /// Execute initialization of assets
        /// </summary>
        public void InitializeAssets()
        {
            //Nessuno in questo progetto
        }
    }
}
