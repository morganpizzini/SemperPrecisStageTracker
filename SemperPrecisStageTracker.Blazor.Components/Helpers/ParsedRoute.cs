using System.Linq;
using System.Text.RegularExpressions;
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
                Console.WriteLine(key);
                url = Regex.Replace(url, $"{{{key}(:.+?)?}}", values[key].ToString(), RegexOptions.IgnoreCase);
            }

            return url;
        }
    }
}