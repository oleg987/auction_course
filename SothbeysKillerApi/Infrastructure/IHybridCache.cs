namespace SothbeysKillerApi.Infrastructure;

public interface IHybridCache
{
    T? Get<T>(string key);
    void Set<T>(string key, T entry, HybridCacheEntryOptions? options = null);
    void Remove(string key);
}