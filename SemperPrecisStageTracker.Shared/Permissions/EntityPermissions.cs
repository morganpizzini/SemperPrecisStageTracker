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
        [Description("DeleteShooter")]
        DeleteShooter = 2,
        [Description("EditMatch")]
        EditMatch = 3,
        [Description("DeleteMatch")]
        DeleteMatch = 4,
        [Description("EditTeam")]
        EditTeam = 5,
        [Description("DeleteTeam")]
        DeleteTeam = 6,
        [Description("EditAssociation")]
        EditAssociation = 7,
        [Description("DeleteAssociation")]
        DeleteAssociation = 8,
        [Description("EditPlace")]
        EditPlace = 9,
        [Description("DeletePlace")]
        DeletePlace = 10
    }
}