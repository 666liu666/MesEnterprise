using System.Text.Json;
using MesEnterprise.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MesEnterprise.Infrastructure.Caching;

public class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<HybridCacheService> _logger;

    public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache, ILogger<HybridCacheService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            return value;
        }

        var cached = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (cached is null)
        {
            return default;
        }

        try
        {
            value = JsonSerializer.Deserialize<T>(cached);
            if (value is not null)
            {
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(5));
            }
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize cached value for {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        _memoryCache.Set(key, value, absoluteExpiration ?? TimeSpan.FromMinutes(5));
        var serialized = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
        }, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }
}
