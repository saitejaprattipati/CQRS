using Author.Query.Persistence.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    //public interface ICacheService
    //{
    //    Task<List<ImageDTO>> GetAllImagesAsync();
    //    Task<bool> AddImagesAsync(IList<ImageDTO> images);
    //    void ClearImagesCacheAsync(string key);
    //}

    public interface ICacheService<TEntity, TEntityDTO>
    {
        Task<List<TEntityDTO>> GetAllAsync(string key);
        Task<bool> AddAsync(IList<TEntityDTO> entity, string cacheKey);
        Task<bool> ClearCacheAsync(string key);
    }
}
