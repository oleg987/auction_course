using Microsoft.Extensions.Caching.Memory;

namespace SothbeysKillerApi.Infrastructure;

public class HybridCacheMemoryProvider : IHybridCache
{
    private readonly IMemoryCache _cache;

    public HybridCacheMemoryProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public void Set<T>(string key, T entry, HybridCacheEntryOptions? options = null)
    {
        if (options is null)
        {
            _cache.Set(key, entry);
        }
        else
        {
            _cache.Set(key, entry, options);
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}