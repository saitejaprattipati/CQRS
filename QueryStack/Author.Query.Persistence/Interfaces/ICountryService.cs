using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICountryService
    {
        Task<CountryResult> GetAllCountriesAsync(LanguageDTO language,int pageNo,int pageSize);

        Task<CountryDTO> GetCountryAsync(LanguageDTO language, int countryId);
    }
}
