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
        MatchInsertScore = 19

    }
}