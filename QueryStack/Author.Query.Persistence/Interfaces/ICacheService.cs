using Author.Query.Persistence.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICacheService
    {
        Task<List<ImageDTO>> GetAllImagesAsync();
        Task<bool> AddImagesAsync(IList<ImageDTO> images);
        void ClearImagesCacheAsync(string key);
    }
}
