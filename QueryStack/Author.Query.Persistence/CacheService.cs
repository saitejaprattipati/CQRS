using Author.Core.Services.Rediscache;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    //public class CacheService : ICacheService
    //{
    //    private const string _imagesCacheKey = "imagesCacheKey";
    //    private readonly RedisConnect _contextRedis;
    //    private IDatabase _cache;
    //    private readonly IImageService _imageService;

    //    public CacheService(IImageService imageService)
    //    {
    //        _contextRedis = new RedisConnect();
    //        _cache = RedisConnect.Connection.GetDatabase();
    //        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    //    }
    //    public async Task<bool> AddImagesAsync(IList<ImageDTO> images)
    //    {
    //        return await _cache.StringSetAsync(_imagesCacheKey, JsonConvert.SerializeObject(images));
    //    }

    //    public async void ClearImagesCacheAsync(string key)
    //    {
    //        await _cache.KeyDeleteAsync(key);
    //    }

    //    public async Task<List<ImageDTO>> GetAllImagesAsync()
    //    {
    //        var imgList = await _cache.StringGetAsync(_imagesCacheKey);

    //        if (imgList.IsNullOrEmpty)
    //        {
    //            // Get from database
    //            var images = await _imageService.GetAllImagesAsync();
    //            var status = await AddImagesAsync(images);
    //            return images;
    //        }
    //        return JsonConvert.DeserializeObject<List<ImageDTO>>(imgList);
    //    }
    //}

    public class CacheService<TEntity, TEntityDTO> : ICacheService<TEntity, TEntityDTO>
        where TEntity : class
        where TEntityDTO : class
    {
        private readonly RedisConnect _contextRedis;
        private IDatabase _cache;
        private readonly TaxathandDbContext _dbContext;
        private readonly IMapper _mapper;
        public CacheService(TaxathandDbContext dbContext, IMapper mapper)
        {
            _contextRedis = new RedisConnect();
            _cache = RedisConnect.Connection.GetDatabase();
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<bool> AddAsync(IList<TEntityDTO> entity, string cacheKey)
        {
            return await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(entity));
        }

        public async Task<bool> ClearCacheAsync(string key)
        {
            return await _cache.KeyDeleteAsync(key);
        }

        public async Task<List<TEntityDTO>> GetAllAsync(string cacheKey)
        {
            var cacheData = await _cache.StringGetAsync(cacheKey);
            if (cacheData.IsNullOrEmpty)
            {
                // Get data from database
                var dbData = await _dbContext.Set<TEntity>().ProjectTo<TEntityDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
                await AddAsync(dbData, cacheKey);
                return dbData;
            }
            return JsonConvert.DeserializeObject<List<TEntityDTO>>(cacheData);
        }
    }
}
