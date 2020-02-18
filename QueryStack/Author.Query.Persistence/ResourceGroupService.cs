using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class ResourceGroupService : IResourceGroupService
    {

        private readonly TaxathandDbContext _dbContext;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public ResourceGroupService(TaxathandDbContext dbContext, ICommonService commonService, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResourceGroupResult> GetResourceGroupsAsync(int pageNo, int pageSize)
        {
            var languageDetails = _commonService.GetLanguageDetails();

            // By default pick the localLanguage value
            var resourceGroups = await GetAllResourceGroupsDataAsync(languageDetails.LocaleLangId, pageNo, pageSize);

            if (resourceGroups == null)
            {
                return null;
            }

            // If localLanguage data is not available then pull the data based on default language
            if (resourceGroups.ResourceGroups.Count == 0)
            {
                resourceGroups = await GetAllResourceGroupsDataAsync(languageDetails.DefaultLanguageId, pageNo, pageSize);
            }

            return resourceGroups;
        }

        private async Task<ResourceGroupResult> GetAllResourceGroupsDataAsync(int languageId, int pageNo, int pageSize)
        {
            var resourceGroupList = new ResourceGroupResult();

            var resourceGroups = await _dbContext.ResourceGroups.Where(cc => cc.IsPublished.Equals(true))
                                        .OrderByDescending(r => r.ResourceGroupId)
                                        .Skip((pageNo - 1) * 100).Take(pageSize)
                                        .ProjectTo<ResourceGroupDTO>(_mapper.ConfigurationProvider)
                                        .AsNoTracking().ToListAsync();


            resourceGroups = resourceGroups.Where(s => s.LanguageId.Equals(languageId)).ToList();

            if (resourceGroups.Count == 0)
            {
                return null;
            }

            resourceGroupList.ResourceGroups = resourceGroups;

            return resourceGroupList;
        }


        public async Task<ResourceGroupDTO> GetResourceGroupAsync(int resourceGroupId)
        {

            var languageDetails = _commonService.GetLanguageDetails();
            // By default pick the localLanguage value
            var resourceGroup = await GetResourceGroupDetailsAsync(resourceGroupId, languageDetails.LocaleLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (resourceGroup == null)
            {
                resourceGroup = await GetResourceGroupDetailsAsync(resourceGroupId, languageDetails.DefaultLanguageId);
            }

            return resourceGroup;
        }

        private async Task<ResourceGroupDTO> GetResourceGroupDetailsAsync(int resourceGroupId, int languageId)
        {
            var resourceGroup = await _dbContext.ResourceGroups.Where(rg => rg.ResourceGroupId.Equals(resourceGroupId) && rg.IsPublished.Equals(true))
                                                               .ProjectTo<ResourceGroupDTO>(_mapper.ConfigurationProvider)
                                                               .AsNoTracking().ToListAsync();

            return resourceGroup.FirstOrDefault(rg => rg.LanguageId.Equals(languageId));
        }
    }
}
