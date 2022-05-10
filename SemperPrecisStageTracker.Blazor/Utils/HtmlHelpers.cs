namespace SemperPrecisStageTracker.Blazor.Utils
{
    public static class HtmlHelpers
    {
        public static string ParseBoolean(this bool b, string trueSentence, string falseSentence = "") => b ? trueSentence : falseSentence;

    }
}