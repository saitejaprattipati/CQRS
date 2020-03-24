using Author.Query.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICommonService
    {
        //Languages GetLanguageFromLocale(string locale);

        LanguageDTO GetLanguageFromLocale(string locale);

        Task<LanguageDTO> GetLanguageFromLocaleAsync(string locale);

        LanguageDetailsDTO GetLanguageDetails();

        Task<LanguageDTO> GetDefaultLanguageAsync();

        Task<List<LanguageDTO>> GetLanguageListFromLocale(List<LanguageDTO> languageFromCache);
        long GetUnixEpochTime(DateTime date);
        DateTime GetDateTimeFromUnixEpochTime(long value);
    }
}
