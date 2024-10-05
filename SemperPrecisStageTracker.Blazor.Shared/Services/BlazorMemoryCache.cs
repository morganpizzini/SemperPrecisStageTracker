using SemperPrecisStageTracker.Shared.Cache;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class BlazorMemoryCache : ISemperPrecisMemoryCache
    {
        private readonly Dictionary<string,object> _cacheDic = new Dictionary<string, object>();

        public bool GetValue<T>(string key, out T result)
        {
            if (_cacheDic.ContainsKey(key))
            {
                result = (T)_cacheDic[key];
                return true;
            }
            result = default!;
            return false;
        }

        public void RemoveValue(string key)
        {
            _cacheDic.Remove(key);
        }

        public void SetValue<T>(string key, T entity, int size = 1)
        {
            if(_cacheDic.ContainsKey(key))
            {
                _cacheDic[key] = entity;
            }
            else
            {
                _cacheDic.Add(key, entity);
            }
        }
    }
}