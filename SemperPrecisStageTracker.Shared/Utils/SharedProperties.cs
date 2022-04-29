using Microsoft.Extensions.Configuration;

namespace SemperPrecisStageTracker.Shared.Utils
{
    public static class SharedProperties
    {
        public static IConfiguration Configuration { get; set; }
    }
}
