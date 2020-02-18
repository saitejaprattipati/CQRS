using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICountryService
    {
        Task<CountryResult> GetAllCountriesAsync(int pageNo,int pageSize);

        Task<CountryDTO> GetCountryAsync(int countryId);
    }
}
