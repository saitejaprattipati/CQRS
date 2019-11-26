using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class ImageService : IImageService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IMapper _mapper;
        public ImageService(TaxathandDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ImageDTO>> GetAllImagesAsync()
        {
            return await _dbContext.Images.ProjectTo<ImageDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ILookup<int, ImageDTO>> GetImageAsync(IEnumerable<int> imageIds)
        {
            var images = await _dbContext.Images.Where(im => imageIds.Contains(im.ImageId))
                                         .ProjectTo<ImageDTO>(_mapper.ConfigurationProvider)
                                         .ToListAsync();
            return images.ToLookup(img => img.ImageId);
        }

        public async Task<ImageDTO> GetImageDetailsAsync(int imageId)
        {
            return await _dbContext.Images.ProjectTo<ImageDTO>(_mapper.ConfigurationProvider)
                                          .FirstOrDefaultAsync(c => c.CountryId.Equals(imageId));
        }


    }
}
