namespace SemperPrecisStageTracker.Domain.Cache
{
    public static class CacheKeys
    {
        public static string Stats => nameof(Stats);

        public static string ComposeKey(string key, params string[] values)
        {
            if (values == null)
                return key;
            return $"{key}-{string.Join("-", values)}";
        }
    }
}