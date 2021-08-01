using System.ComponentModel;

namespace SemperPrecisStageTracker.Shared.Permissions
{
    public enum EntityPermissions
    {
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