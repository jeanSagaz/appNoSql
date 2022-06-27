using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using appNoSql.Infra.Data.Redis.Interfaces;
using appNoSql.Domain.Core.Models;

namespace appNoSql.Infra.Data.Redis.Repository
{
    public class RepositoryRedis<T> : IRepositoryRedis<T> where T : Entity
    {
        private readonly IDistributedCache _cache;

        public RepositoryRedis(IDistributedCache cache)
        {
            _cache = cache;
        }

        private DistributedCacheEntryOptions SetDistributedCacheEntryOptions(TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;

            return options;
        }

        public virtual async Task Set(string id, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = SetDistributedCacheEntryOptions(absoluteExpireTime, unusedExpireTime);

            var jsonData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(id, jsonData, options);
        }

        public virtual async Task<bool> Update(string id, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = SetDistributedCacheEntryOptions(absoluteExpireTime,unusedExpireTime);

            var result = await GetById(id);
            if (result is null)            
                return false;

            var jsonData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(id, jsonData, options);
            return true;
        }

        public virtual async Task<bool> Remove(string id)
        {
            var result = await GetById(id);

            if (result is null)
                return false;

            await _cache.RemoveAsync(id);
            return true;
        }

        public virtual async Task<T> GetById(string id)
        {
            var jsonData = await _cache.GetStringAsync(id);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
