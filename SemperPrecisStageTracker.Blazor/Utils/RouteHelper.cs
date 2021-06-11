using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public class ParsedRoute
    {
        public string Template { get; private set; }
        public string[] Parameters { get; private set; }

        public ParsedRoute(string template)
        {
            this.Template = template;

            this.Parameters = Regex.Matches(template, "{(\\w+)(:.+?)?}")
                .Select(x => x.Groups[1].Value)
                .ToArray();
        }

        public string Render(RouteValueDictionary values)
        {
            string url = this.Template;

            foreach (var key in values.Keys)
            {
                url = Regex.Replace(url, $"{{{key}(:.+?)?}}", values[key].ToString(), RegexOptions.IgnoreCase);
            }

            return url;
        }
    }

    public static class RouteHelper
    {
        private static IEnumerable<ParsedRoute> ParseRoutes<TComponent>() where TComponent : ComponentBase
        {
            var type = typeof(TComponent);
            var result = type.GetCustomAttributes(typeof(RouteAttribute), inherit: false)
                .OfType<RouteAttribute>()
                .Select(x => new ParsedRoute(x.Template));

            return result;
        }

        public static string GetUrl<TComponent>(object parameters = null)
            where TComponent : ComponentBase
        {
            var properties = new RouteValueDictionary(parameters);

            var result = ParseRoutes<TComponent>()
                // check input parameters count
                .Where(x => x.Parameters.Length == properties.Count)
                // check input parameters names
                .Where(x => x.Parameters.All(parameter => properties.ContainsKey(parameter)))
                .Select(x => x.Render(properties))
                .FirstOrDefault();

            return result;
        }
    }
}