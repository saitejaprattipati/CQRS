using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Author.Query.Persistence
{
    public class CountryService : ICountryService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICacheService<Images, ImageDTO> _cacheService;

        public CountryService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings,
            ICacheService<Images, ImageDTO> cacheService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _appSettings = appSettings;
        }

        public async Task<CountryResult> GetAllCountriesAsync(LanguageDTO language, int pageNo, int pageSize)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var countries = await GetAllCountriesDataAsync(localeLangId, pageNo, pageSize);

            // If localLanguage data is not available then pull the data based on default language
            if (countries.Countries.Count == 0)
            {
                countries = await GetAllCountriesDataAsync(dftLanguageId, pageNo, pageSize);
            }

            return countries;
        }

        public async Task<CountryDTO> GetCountryAsync(LanguageDTO language, int countryId)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);
            //var country = new CountryDTO();

            // By default pick the localLanguage value
            var country = await GetCountryDetailsAsync(countryId, localeLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (country == null)
            {
                country = await GetCountryDetailsAsync(countryId, dftLanguageId);
            }

            return country;
        }

        private async Task<CountryDTO> GetCountryDetailsAsync(int countryId, int languageId)
        {
            var images = await _cacheService.GetAllAsync("imagesCacheKey");

            var country = await _dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.CountryId.Equals(countryId) &&
                                                            c.IsPublished.Equals(true) && c.LanguageId.Equals(languageId));

            if (country == null)
            {
                return null;
            }

            var countryDTO = new CountryDTO
            {
                Uuid = country.CountryId,
                PNGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(country.PNGImageId)).FilePath,
                SVGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(country.SVGImageId)).FilePath,
                DisplayName = country.DisplayName,
                DisplayNameShort = country.DisplayName,
                Name = Helper.ReplaceChars(country.DisplayName),
                Path = Helper.ReplaceChars(country.DisplayName),
                CompleteResponse = true
            };

            return countryDTO;
        }

        private async Task<CountryResult> GetAllCountriesDataAsync(int languageId, int pageNo, int pageSize)
        {
            var countryList = new CountryResult();
            var images = await _cacheService.GetAllAsync("imagesCacheKey");

            //var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
            //                        .Select(c => new { c.CountryId, c.DisplayName, c.PNGImageId, c.SVGImageId, })
            //                        .Skip((pageNo - 1) * 100).Take(pageSize).AsNoTracking().ToListAsync();
            var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true))
                                    .Select(c => new { c.CountryId, c.DisplayName, c.PNGImageId, c.SVGImageId, c.LanguageId })
                                    .OrderByDescending(s=>s.CountryId)
                                    .Skip((pageNo - 1) * 100).Take(pageSize).AsNoTracking().ToListAsync();
            countries = countries.Where(s => s.LanguageId.Equals(languageId)).ToList();

            if (countries.Count == 0)
            {
                return null;
            }

            countryList.Countries.AddRange(countries.Select(co => new CountryDTO
            {
                Uuid = co.CountryId,
                PNGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(co.PNGImageId)).FilePath,
                SVGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(co.SVGImageId)).FilePath,
                DisplayName = co.DisplayName,
                DisplayNameShort = co.DisplayName,
                Name = Helper.ReplaceChars(co.DisplayName),
                Path = Helper.ReplaceChars(co.DisplayName),
                CompleteResponse = true
            }));

            return countryList;
        }
    }
}
