namespace SemperPrecisStageTracker.Domain.Cache
{
    public interface ISemperPrecisMemoryCache
    {
        T GetValue<T>(string key);

        void SetValue<T>(string key, T entity , int size = 1);
        void RemoveValue(string key);
    }
}