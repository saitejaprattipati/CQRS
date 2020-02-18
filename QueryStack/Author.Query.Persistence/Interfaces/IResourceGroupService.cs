using Author.Query.Persistence.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface IResourceGroupService
    {
        Task<ResourceGroupResult> GetResourceGroupsAsync(int pageNo, int pageSize);

        Task<ResourceGroupDTO> GetResourceGroupAsync(int resourceGroupId);
    }
}
