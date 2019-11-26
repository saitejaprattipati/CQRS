using Author.Core.Services.Rediscache;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class CacheService : ICacheService
    {
        private const string _imagesCacheKey = "imagesCacheKey";
        private readonly RedisConnect _contextRedis;
        private IDatabase _cache;
        private readonly IImageService _imageService;

        public CacheService(IImageService imageService)
        {
            _contextRedis = new RedisConnect();
            _cache = RedisConnect.Connection.GetDatabase();
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }
        public async Task<bool> AddImagesAsync(IList<ImageDTO> images)
        {
           return await _cache.StringSetAsync(_imagesCacheKey, JsonConvert.SerializeObject(images));
        }

        public async void ClearImagesCacheAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task<List<ImageDTO>> GetAllImagesAsync()
        {
            var imgList = await _cache.StringGetAsync(_imagesCacheKey);

            if (imgList.IsNullOrEmpty)
            {
                // Get from database
                var images = await _imageService.GetAllImagesAsync();
                var status = await AddImagesAsync(images);
                return images;
            }
            return JsonConvert.DeserializeObject<List<ImageDTO>>(imgList);
        }
    }
}
