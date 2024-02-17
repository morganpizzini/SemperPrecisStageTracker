using System.Text.Json;
using System.Text.RegularExpressions;

namespace SemperPrecisStageTracker.Shared.Utils;
public static class CloneEntity
{
    public static T Clone<T>(this T source)
    {
        var serialized = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialized);
    }
}
public static class StringExtensions
{
    public static bool MatchesRegexPattern(this string input, string pattern)
    {
        // Use Regex.IsMatch to check if the input string matches the pattern
        return Regex.IsMatch(input, pattern);
    }
}
