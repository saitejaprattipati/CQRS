using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICountryGroupService
    {
        Task<CountryGroupResult> GetCountryGroupsAsync(LanguageDTO language, int pageNo, int pageSize);

        Task<CountryGroupDTO> GetCountryGroupAsync(LanguageDTO language, int countryGroupId);
    }
}
