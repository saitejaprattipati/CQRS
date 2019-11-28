using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface IImageService 
    {
        Task<ILookup<int, ImageDTO>> GetImageAsync(IEnumerable<int> imageIds);

        Task<ImageDTO> GetImageDetailsAsync(int imageId);

        Task<List<ImageDTO>> GetAllImagesAsync();
    }
}
