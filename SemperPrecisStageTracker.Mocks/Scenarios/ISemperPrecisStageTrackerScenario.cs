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
        IList<Shooter> Shooters { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        IList<ShooterData> ShooterDatas { get; set; }

        /// <summary>
        /// List of Shooter Team Payment
        /// </summary>
        IList<ShooterTeamPayment> ShooterTeamPayments { get; set; }

        /// <summary>
        /// List of shooter
        /// </summary>
        IList<TeamHolder> TeamHolders { get; set; }


        /// <summary>
        /// List of stage
        /// </summary>
        IList<Stage> Stages { get; set; }

        /// <summary>
        /// List of shooter stage
        /// </summary>
        IList<ShooterStage> ShooterStages { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<GroupShooter> GroupShooters { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<ShooterMatch> ShooterMatches { get; set; }

        /// <summary>
        /// List of group shooters
        /// </summary>
        IList<ShooterSOStage> ShooterSOStages { get; set; }

        /// <summary>
        /// List of team shooters
        /// </summary>
        IList<ShooterTeam> ShooterTeams { get; set; }

        /// <summary>
        /// List of teams
        /// </summary>
        IList<Team> Teams { get; set; }

        /// <summary>
        /// List of association shooters
        /// </summary>
        IList<ShooterAssociation> ShooterAssociations { get; set; }

        /// <summary>
        /// List of association shooters info
        /// </summary>
        IList<ShooterAssociationInfo> ShooterAssociationInfos { get; set; }

        /// <summary>
        /// List of notification subscription
        /// </summary>
        IList<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// List of place
        /// </summary>
        IList<Place> Places { get; set; }

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
    }
}
