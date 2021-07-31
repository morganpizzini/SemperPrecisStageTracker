using System.Collections.Generic;
using System.Threading.Tasks;
using DnetIndexedDb;

namespace SemperPrecisStageTracker.Blazor
{
    public static class IndexedDbInteropExtensions
    {
        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ValueTask<string> AddItems<TEntity>(this IndexedDbInterop idi, List<TEntity> items)
        {
            return idi.AddItems(typeof(TEntity).Name, items);
        }

        public static ValueTask<string> UpdateItems<TEntity>(this IndexedDbInterop idi, List<TEntity> items)
        {
            return idi.UpdateItems(typeof(TEntity).Name, items);
        }
        public static ValueTask<TEntity> GetByKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.GetByKey<TKey, TEntity>(typeof(TEntity).Name, key);
        }
        public static ValueTask<string> DeleteByKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.DeleteByKey(typeof(TEntity).Name, key);
        }
        public static ValueTask<string> DeleteAll<TEntity>(this IndexedDbInterop idi)
        {
            return idi.DeleteAll(typeof(TEntity).Name);
        }
        public static ValueTask<List<TEntity>> GetAll<TEntity>(this IndexedDbInterop idi)
        {
            return idi.GetAll<TEntity>(typeof(TEntity).Name);
        }
        public static ValueTask<List<TEntity>> GetRange<TKey, TEntity>(this IndexedDbInterop idi, TKey lowerBound, TKey upperBound)
        {
            return idi.GetRange<TKey, TEntity>(typeof(TEntity).Name, lowerBound, upperBound);
        }
        public static ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(this IndexedDbInterop idi, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return idi.GetByIndex<TKey, TEntity>(typeof(TEntity).Name, lowerBound, upperBound, dbIndex, isRange);
        }
        public static ValueTask<TIndex> GetMaxIndex<TIndex, TEntity>(this IndexedDbInterop idi, string dbIndex)
        {
            return idi.GetMaxIndex<TIndex>(typeof(TEntity).Name, dbIndex);
        }
        public static ValueTask<TKey> GetMaxKey<TKey, TEntity>(this IndexedDbInterop idi)
        {
            return idi.GetMaxKey<TKey>(typeof(TEntity).Name);
        }
        public static ValueTask<TIndex> GetMinIndex<TIndex, TEntity>(this IndexedDbInterop idi, string dbIndex)
        {
            return idi.GetMinIndex<TIndex>(typeof(TEntity).Name, dbIndex);
        }
        public static ValueTask<TKey> GetMinKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.GetMinKey<TKey>(typeof(TEntity).Name);
        }
    }
}