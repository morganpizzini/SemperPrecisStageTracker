namespace SemperPrecisStageTracker.Shared.Cache
{
    public interface ISemperPrecisMemoryCache
    {
        bool GetValue<T>(string key, out T result);

        void SetValue<T>(string key, T entity, int size = 1);
        void RemoveValue(string key);
    }
}