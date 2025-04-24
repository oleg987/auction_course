using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace SothbeysKillerApi.Infrastructure;

public class HybridCacheEntryOptions
{
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }

    public static implicit operator MemoryCacheEntryOptions(HybridCacheEntryOptions options)
    {
        return new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = options.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = options.SlidingExpiration
        };
    }
    
    public static implicit operator DistributedCacheEntryOptions(HybridCacheEntryOptions options)
    {
        return new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = options.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = options.SlidingExpiration
        };
    }
}