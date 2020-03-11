using Author.Core.Services.Rediscache;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
    public class CacheService<TEntity, TEntityDTO> : ICacheService<TEntity, TEntityDTO>
        where TEntity : class
        where TEntityDTO : class
    {
        private IDatabase _cache;
        public CacheService()
        {
            _cache = RedisConnect.Connection.GetDatabase();
        }
        public async Task<bool> AddAsync(IList<TEntityDTO> entity, string cacheKey)
        {
            return await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(entity));
        }

        public async Task<bool> ClearCacheAsync(string key)
        {
            return await _cache.KeyDeleteAsync(key);
        }
    }
    public interface ICacheService<TEntity, TEntityDTO>
    {
        Task<bool> AddAsync(IList<TEntityDTO> entity, string cacheKey);
        Task<bool> ClearCacheAsync(string key);
    }
}
