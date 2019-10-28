using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICountryService
    {
        Task<CountryResult> GetAllCountriesAsync(string locale);
    }
}
