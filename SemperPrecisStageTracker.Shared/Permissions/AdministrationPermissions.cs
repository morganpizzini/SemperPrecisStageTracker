using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Description("ManageStages")]
        ManageStages = 4,
        [Description("CreateMatches")]
        CreateMatches = 5,
        [Description("CreateShooters")]
        CreateShooters = 6,
        [Description("CreateTeams")]
        CreateTeams = 7,
        [Description("CreateStages")]
        CreateStages = 8
    }
    // singularity permissions
}
