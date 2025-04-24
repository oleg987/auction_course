using Microsoft.Extensions.Caching.Distributed;
using SothbeysKillerApi.Extensions;

namespace SothbeysKillerApi.Infrastructure;

public class HybridCacheDistributedProvider : IHybridCache
{
    private readonly IDistributedCache _cache;

    public HybridCacheDistributedProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        return _cache.GetGeneric<T>(key);
    }

    public void Set<T>(string key, T entry, HybridCacheEntryOptions? options = null)
    {
        if (options is null)
        {
            _cache.SetGeneric(key, entry);
        }
        else
        {
            _cache.SetGeneric(key, entry, options);
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}