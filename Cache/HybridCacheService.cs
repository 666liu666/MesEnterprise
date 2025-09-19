using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace MesEnterprise.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, Func<Task<T?>> loader, TimeSpan? ttl = null);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
    Task RemoveAsync(string key);
}

public sealed class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabase _redisDatabase;

    public HybridCacheService(IMemoryCache memoryCache, IRedisConnector connector)
    {
        _memoryCache = memoryCache;
        _redisDatabase = connector.GetConnection().GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, Func<Task<T?>> loader, TimeSpan? ttl = null)
    {
        if (_memoryCache.TryGetValue(key, out T? memoryValue))
        {
            return memoryValue;
        }

        var cached = await _redisDatabase.StringGetAsync(key);
        if (cached.HasValue)
        {
            var deserialized = JsonSerializer.Deserialize<T>(cached!);
            _memoryCache.Set(key, deserialized, ttl ?? TimeSpan.FromMinutes(5));
            return deserialized;
        }

        var loaded = await loader();
        if (loaded is not null)
        {
            await SetAsync(key, loaded, ttl);
        }

        return loaded;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        _memoryCache.Set(key, value, ttl ?? TimeSpan.FromMinutes(5));
        var serialized = JsonSerializer.Serialize(value);
        await _redisDatabase.StringSetAsync(key, serialized, ttl ?? TimeSpan.FromMinutes(5));
    }

    public async Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        await _redisDatabase.KeyDeleteAsync(key);
    }
}
