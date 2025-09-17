using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? absoluteExpireTime = null);
    void Remove(string key);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T value);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        _memoryCache.Set(key, value, expiration ?? TimeSpan.FromMinutes(5));
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
