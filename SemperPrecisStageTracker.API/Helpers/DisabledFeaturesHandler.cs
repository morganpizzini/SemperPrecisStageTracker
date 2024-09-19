using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace SemperPrecisStageTracker.API.Helpers
{

    public class DisabledFeaturesHandler : IDisabledFeaturesHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
        {
            var featuresDescription = string.Join(",", features);
            var verb = features.Count() > 1 ? "are" : "is";
            var message = $"The following feature(s) {verb} not available: {featuresDescription}";
            context.Result = new BadRequestObjectResult(message);
            return Task.CompletedTask;
        }
    }
}
