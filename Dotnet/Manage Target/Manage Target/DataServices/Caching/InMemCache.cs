
using Microsoft.Extensions.Caching.Memory;

namespace Manage_Target.DataServices.Caching
{
    public class InMemCache
    {
        private MemoryCache Cache { get; } = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        private MemoryCacheEntryOptions GetCacheEntryOption()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(15));
            cacheEntryOptions.SetSize(1);
            return cacheEntryOptions;
        }
        public async Task<IEnumerable<T>> GetList<T>(string cacheKey, Func<Task<List<T>>> func)
        {
            if (!Cache.TryGetValue(cacheKey, out List<T> items))
            {
                items = await func();
                if (items != null)
                {
                    Cache.Set(cacheKey, items, GetCacheEntryOption());
                }
            }
            return items;
        }
        public void ClearCache(string cacheKey)
        {
            Cache.Remove(cacheKey);
            //Cache.Clear();
        }
    }
    public static class CacheKeys
    {
        public const string Items = "1. List Item";
        public const string Tasks = "2. List Task";
    }
}
