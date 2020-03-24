using Author.Query.Persistence.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICountryService
    {
        Task<CountryResult> GetAllCountriesAsync();

        Task<CountryDTO> GetCountryAsync(int countryId);

        Task<CountryResult> GetCountriesByIdsAsync(List<int> countryIds, int defaultLanguageId, List<int> localeLanguageIdList, List<ImageDTO> imagesFromCache);
    }
}
