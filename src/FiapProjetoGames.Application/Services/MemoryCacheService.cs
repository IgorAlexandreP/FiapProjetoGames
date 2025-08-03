using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace FiapProjetoGames.Application.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, DateTime> _expirationTimes;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _expirationTimes = new ConcurrentDictionary<string, DateTime>();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (await ExistsAsync(key))
            {
                var value = _memoryCache.Get<string>(key);
                if (value != null)
                {
                    return JsonSerializer.Deserialize<T>(value);
                }
            }
            return default(T);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
                _expirationTimes.AddOrUpdate(key, DateTime.UtcNow.Add(expiration.Value), (k, v) => DateTime.UtcNow.Add(expiration.Value));
            }

            _memoryCache.Set(key, serializedValue, options);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            _expirationTimes.TryRemove(key, out _);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                if (_expirationTimes.TryGetValue(key, out var expirationTime))
                {
                    if (DateTime.UtcNow > expirationTime)
                    {
                        await RemoveAsync(key);
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (await ExistsAsync(key))
            {
                return await GetAsync<T>(key);
            }

            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }
    }
} 