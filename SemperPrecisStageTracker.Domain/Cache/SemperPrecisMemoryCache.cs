using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace SemperPrecisStageTracker.Domain.Cache
{
    public class SemperPrecisMemoryCache : ISemperPrecisMemoryCache
    {
        private MemoryCache _cache { get; set; }

        public SemperPrecisMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024,

            });
        }

        public T GetValue<T>(string key)
        {
            _cache.TryGetValue(key, out var cacheEntry);
            if (cacheEntry is T tmp)
            {
                return tmp;
            }
            return default;
        }

        public void SetValue<T>(string key, T entity, int size = 1)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(size)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            // Save data in cache.
            _cache.Set(key, entity, cacheEntryOptions);
        }

        public void RemoveValue(string key)
        {
            _cache.Remove(key);
        }
    }
}
