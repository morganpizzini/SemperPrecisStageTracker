using System.ComponentModel;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    // ranged permissions
    public enum AdministrationPermissions
    {
        [Description("ManageMatches")]
        ManageMatches = 1,
        [Description("ManageShooters")]
        ManageShooters = 2,
        [Description("ManageTeams")]
        ManageTeams = 3,
        [Description("ManageAssociations")]
        ManageAssociations = 9,
        [Description("ManagePlaces")]
        ManagePlaces = 11,
        [Description("ManageStages")]
        ManageStages = 4,
        [Description("CreateMatches")]
        CreateMatches = 5,
        [Description("CreateShooters")]
        CreateShooters = 6,
        [Description("CreateTeams")]
        CreateTeams = 7,
        [Description("CreateStages")]
        CreateStages = 8,
        [Description("CreateAssociations")]
        CreateAssociations = 10,
        [Description("CreatePlaces")]
        CreatePlaces = 12,
        [Description("ShowShooters")]
        ShowShooters = 13
    }
    // singularity permissions
}
