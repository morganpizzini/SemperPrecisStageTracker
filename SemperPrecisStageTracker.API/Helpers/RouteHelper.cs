using System.Linq;
using Microsoft.AspNetCore.Components;

namespace SemperPrecisStageTracker.API.Helpers
{
    public static class RouteHelper
    {
        public static string GetUrl<TComponent>() where TComponent : ComponentBase
        {
            var type = typeof(TComponent);
            var att = type.GetCustomAttributes(typeof(RouteAttribute), inherit: false)
                .OfType<RouteAttribute>()
                .SingleOrDefault();

            return att?.Template;
        }
    }
}