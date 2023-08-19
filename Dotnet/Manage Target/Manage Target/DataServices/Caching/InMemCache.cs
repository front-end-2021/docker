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
        public async Task<IEnumerable<T>> GetList<T>(Func<Task<List<T>>> func)
        {
            var cacheKey = GetCacheKey<T>();
            if (string.IsNullOrEmpty(cacheKey)) return new List<T>();

            if (Cache.TryGetValue(cacheKey, out IEnumerable<T> list)) return list ?? new List<T>();

            list = await func();
            if (list == null) return new List<T>();

            Cache.Set(cacheKey, list, GetCacheEntryOption());
            return list;
        }
        private string GetCacheKey<T>()
        {
            switch (typeof(T).FullName)
            {
                case "Manage_Target.Models.Item":
                    return CacheKeys.Items;
                case "Manage_Target.Models.Task":
                    return CacheKeys.Tasks;
                default:
                    return string.Empty;
            }
        }

        public void ClearCache<T>()
        {
            string cacheKey = GetCacheKey<T>();
            if(string.IsNullOrEmpty(cacheKey)) return;

            Cache.Remove(cacheKey);
            //Cache.Clear();
        }
    }
    public static class CacheKeys
    {
        public const string Items = "1. List Item";
        public const string Tasks = "2. List Task";
        public const string Entry = "_Entry";
    }
}
