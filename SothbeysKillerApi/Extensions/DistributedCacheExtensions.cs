using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SothbeysKillerApi.Extensions;

public static class DistributedCacheExtensions
{
    public static T? GetGeneric<T>(this IDistributedCache cache, string key)
    {
        var bytes = cache.Get(key);

        if (bytes == null)
        {
            return default;
        }
        
        var json = Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<T>(json);
    }
    
    public static void SetGeneric<T>(this IDistributedCache cache, string key, T entry, DistributedCacheEntryOptions? options = null)
    {
        var json = JsonSerializer.Serialize(entry);

        var bytes = Encoding.UTF8.GetBytes(json);

        if (options is null)
        {
            cache.Set(key, bytes);
        }
        else
        {
            cache.Set(key, bytes, options);
        }
    }
}