using System;
using System.Collections.Generic;
using System.Linq;

namespace SemperPrecisStageTracker.Blazor.Utils
{
    public static class CommonVariables
    {
        public static string ClientSettingsKey => nameof(ClientSettingsKey);
    }
    public static class HtmlHelpers
    {
        public static string ParseBoolean(this bool b, string trueSentence, string falseSentence = "") =>b ? trueSentence : falseSentence;
     
    }
    public static class EnumUtil {
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}