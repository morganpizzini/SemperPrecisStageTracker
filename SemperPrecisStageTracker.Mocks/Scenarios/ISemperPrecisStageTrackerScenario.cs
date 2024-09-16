using System.Collections.Generic;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Mocks.Scenarios;

namespace SemperPrecisStageTracker.Mocks.Scenarios
{
    /// <summary>
    /// Interface for application scenario
    /// </summary>
    public interface ISemperPrecisStageTrackerScenario : IScenario
    {
        /// <summary>
        /// List of groups
        /// </summary>
        IList<Group> Groups { get; set; }

        /// <summary>
        /// List of match
        /// </summary>
        IList<Match> Matches { get; set; }

        /// <summary>
        /// List of match
        /// </summary>
        IList<Association> Associations { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        IList<User> Users { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        IList<UserData> ShooterDatas { get; set; }

        /// <summary>
        /// List of Shooter Team Payment
        /// </summary>
        IList<TeamPayment> ShooterTeamPayments { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        IList<TeamHolder> TeamHolders { get; set; }


        /// <summary>
        /// List of stage
        /// </summary>
        IList<Stage> Stages { get; set; }

        /// <summary>
        /// List of stage
        /// </summary>
        IList<StageString> StageStrings { get; set; }

        /// <summary>
        /// List of shooter stage
        /// </summary>
        IList<UserStageString> ShooterStages { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<GroupUser> GroupShooters { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<UserMatch> ShooterMatches { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<UserSOStage> ShooterSOStages { get; set; }

        /// <summary>
        /// List of team shooters
        /// </summary>
        IList<UserTeam> ShooterTeams { get; set; }

        /// <summary>
        /// List of teams
        /// </summary>
        IList<Team> Teams { get; set; }

        /// <summary>
        /// List of association shooters
        /// </summary>
        IList<UserAssociation> ShooterAssociations { get; set; }

        /// <summary>
        /// List of association shooters info
        /// </summary>
        IList<UserAssociationInfo> ShooterAssociationInfos { get; set; }

        /// <summary>
        /// List of notification subscription
        /// </summary>
        IList<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// List of place
        /// </summary>
        IList<Place> Places { get; set; }

        /// <summary>
        /// List of place
        /// </summary>
        IList<PlaceData> PlaceDatas { get; set; }

        /// <summary>
        /// List of contact
        /// </summary>
        IList<Contact> Contacts { get; set; }
        
        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<Permission> Permissions { get; set; }

        
        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<PermissionRole> PermissionRoles { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<Role> Roles { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<UserRole> UserRoles { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<UserPermission> UserPermissions { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<PermissionGroup> PermissionGroups { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<UserPermissionGroup> UserPermissionGroups { get; set; }

        /// <summary>
        /// List of entity permissions
        /// </summary>
        IList<PermissionGroupRole> PermissionGroupRoles { get; set; }

        /// <summary>
        /// List of team reminder
        /// </summary>
        IList<TeamReminder> TeamReminders { get; set; }

        /// <summary>
        /// List of team reminder
        /// </summary>
        IList<PaymentType> PaymentTypes { get; set; }
        IList<Bay> Bays { get; set; }
        IList<Schedule> Schedules { get; set; }
        IList<BaySchedule> BaySchedules { get; set; }
        IList<Reservation> Reservations { get; set; }

    }
}
