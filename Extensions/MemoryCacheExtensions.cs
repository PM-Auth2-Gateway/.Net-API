using System;
using Microsoft.Extensions.Caching.Memory;
#pragma warning disable 1591

namespace PMAuth.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static TItem Peek<TItem>(this IMemoryCache cache, object key)
        {
            bool isValuePresents = cache.TryGetValue(key, out TItem item);
            if (isValuePresents)
            {
                cache.Set(key, item, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                    SlidingExpiration =TimeSpan.FromMinutes(1)
                });
                return item;
            }
            else
            {
                return default;
            }
        }
    }
}