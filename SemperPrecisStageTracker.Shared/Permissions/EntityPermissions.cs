using System.Collections.Generic;
using System.ComponentModel;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public enum Permissions
    {
        // admin
        [Description("ManageMatches")]
        ManageMatches = 901,
        [Description("ManageShooters")]
        ManageShooters = 902,
        [Description("ManageTeams")]
        ManageTeams = 903,
        [Description("ManageAssociations")]
        ManageAssociations = 904,
        [Description("ManagePlaces")]
        ManagePlaces = 905,
        [Description("ManageStages")]
        ManageStages = 906,
        [Description("CreateMatches")]
        CreateMatches = 907,
        [Description("CreateShooters")]
        CreateShooters = 908,
        [Description("CreateTeams")]
        CreateTeams = 909,
        [Description("CreateStages")]
        CreateStages = 910,
        [Description("CreateAssociations")]
        CreateAssociations = 911,
        [Description("CreatePlaces")]
        CreatePlaces = 912,
        [Description("ShowShooters")]
        ShowShooters = 913,
        [Description("ManageMatches")]
        ManagePermissions = 912,

        // entity
        [Description("EditShooter")]
        EditShooter = 1,
        [Description("EditMatch")]
        EditMatch = 14,
        [Description("EditTeam")]
        EditTeam = 5,
        [Description("EditAssociation")]
        EditAssociation = 7,
        [Description("EditPlace")]
        EditPlace = 9,

        [Description("TeamEditShooters")]
        TeamEditShooters = 15,
        [Description("TeamEditPayment")]
        TeamEditPayment = 16,
        [Description("MatchManageGroups")]
        MatchManageGroups = 17,
        [Description("MatchManageStageSO")]
        MatchManageStageSO = 18,
        [Description("MatchInsertScore")]
        MatchInsertScore = 19,
        [Description("MatchManageMD")]
        MatchManageMD = 20,
        [Description("MatchManageStages")]
        MatchManageStages= 21,
        [Description("MatchHandling")]
        MatchHandling = 22
    }

    public interface IPermissionInterface
    {
        IPermissionInterface ManageMatches { get; }
        IPermissionInterface ManageShooters { get; }
        IPermissionInterface ManageTeams { get; }
    }
    public class PermissionHandler : IPermissionInterface
    {
        private readonly IList<Permissions> permissions = new List<Permissions>();
        public IPermissionInterface AddPermission(Permissions perm)
        {
            permissions.Add(perm);
            return this;
        }
        public IList<Permissions> List => permissions;

        public IPermissionInterface ManageMatches => AddPermission(Permissions.ManageMatches);
        public IPermissionInterface ManageShooters => AddPermission(Permissions.ManageShooters);
        public IPermissionInterface ManageTeams => AddPermission(Permissions.ManageTeams);
        
        public override string ToString()
        {
            return string.Join(",", permissions);
        }
    }

    public static class PermissionConstructor
    {
        public static IPermissionInterface ManageMatches => new PermissionHandler().ManageMatches;
        public static IPermissionInterface ManageShooters => new PermissionHandler().ManageShooters;
        public static IPermissionInterface ManageTeams => new PermissionHandler().ManageTeams;
    }
}