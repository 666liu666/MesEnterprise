using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Text.Json;

namespace MesEnterprise.Cache
{
   
        public interface ICacheService { Task<T?> GetAsync<T>(string key, Func<Task<T>> loader, TimeSpan? ttl = null); Task SetAsync<T>(string key, T value, TimeSpan? ttl = null); Task RemoveAsync(string key); }
        public class HybridCacheService : ICacheService
        {
            private readonly IMemoryCache _memory;
            private readonly IDatabase _redis;
            public HybridCacheService(IMemoryCache memory, IRedisConnector connector) { _memory = memory; _redis = connector.GetConnection().GetDatabase(); }

            public async Task<T?> GetAsync<T>(string key, Func<Task<T>> loader, TimeSpan? ttl = null)
            {
                if (_memory.TryGetValue(key, out T mem)) return mem;
                var val = await _redis.StringGetAsync(key);
                if (val.HasValue) { var v = JsonSerializer.Deserialize<T>(val!); _memory.Set(key, v, ttl ?? TimeSpan.FromSeconds(30)); return v; }
                var loaded = await loader();
                if (loaded != null) { await SetAsync(key, loaded, ttl); }
                return loaded;
            }
            public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
            {
                _memory.Set(key, value, ttl ?? TimeSpan.FromSeconds(30));
                var json = JsonSerializer.Serialize(value);
                await _redis.StringSetAsync(key, json, ttl);
            }
            public async Task RemoveAsync(string key) { _memory.Remove(key); await _redis.KeyDeleteAsync(key); }
        }
    
}
