using System.Text.Json;

namespace SemperPrecisStageTracker.Shared.Utils;
public static class CloneEntity
{
    public static T Clone<T>(this T source)
    {
        var serialized = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialized);
    }
}
