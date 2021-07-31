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
        DeleteMatch = 4
    }
}