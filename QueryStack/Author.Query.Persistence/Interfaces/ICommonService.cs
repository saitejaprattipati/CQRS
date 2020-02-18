using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICommonService
    {
        //Languages GetLanguageFromLocale(string locale);

        LanguageDTO GetLanguageFromLocale(string locale);

        Task<LanguageDTO> GetLanguageFromLocaleAsync(string locale);

        LanguageDetailsDTO GetLanguageDetails();
    }
}
