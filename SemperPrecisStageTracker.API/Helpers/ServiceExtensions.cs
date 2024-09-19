using Microsoft.FeatureManagement;

namespace SemperPrecisStageTracker.API.Helpers
{
    public static class ServiceExtensions
    {
        public static bool IsFeatureEnabled(this IConfiguration configuration, string feature)
        {
            var featureServices = new ServiceCollection();
            featureServices.AddFeatureManagement(configuration);
            using var provider = featureServices.BuildServiceProvider();
            var manager = provider.GetRequiredService<IFeatureManager>();

            return manager.IsEnabledAsync(feature).GetAwaiter().GetResult();
        }
        public static IApplicationBuilder UseTimer(this IApplicationBuilder app) => app.UseMiddleware<TimerMiddleware>();
    }
}
