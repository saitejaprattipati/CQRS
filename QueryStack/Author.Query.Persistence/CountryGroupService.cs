using Author.Core.Framework;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class CountryGroupService : ICountryGroupService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;

        public CountryGroupService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appSettings = appSettings;
        }

        public async Task<CountryGroupDTO> GetCountryGroupAsync(LanguageDTO language, int countryGroupId)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var countryGroup = await GetCountryGroupDataAsync(countryGroupId, localeLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (countryGroup == null)
            {
                countryGroup = await GetCountryGroupDataAsync(countryGroupId, dftLanguageId);
            }

            return countryGroup;
        }

        private async Task<CountryGroupDTO> GetCountryGroupDataAsync(int countryGroupId, int languageId)
        {
            // Get all countries 
            var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
                                    .Select(c => new { c.CountryId, c.DisplayName, c.DisplayNameShort })
                                    .AsNoTracking().ToListAsync();

            // Get countrygroup
            var countryGroup = await _dbContext.CountryGroups.AsNoTracking().FirstOrDefaultAsync(cg => cg.CountryGroupId.Equals(countryGroupId) &&
                                        cg.IsPublished.Equals(true) && cg.LanguageId.Equals(languageId));

            if (countryGroup == null)
            {
                return null;
            }

            var countryGroupDTO = new CountryGroupDTO
            {
                CountryGroupId= countryGroup.CountryGroupId,
                GroupName = countryGroup.GroupName,
                CountryGroupAssociatedCountries = countries.Where(c => countryGroup.AssociatedCountryIds.Any(s => s.IdVal == c.CountryId))
                                                  .Select(co => new CountryDTO
                                                  {
                                                      Uuid = co.CountryId,
                                                      DisplayName = co.DisplayName,
                                                      DisplayNameShort = co.DisplayNameShort
                                                  }).ToList()
            };

            return countryGroupDTO;
        }

        public async Task<CountryGroupResult> GetCountryGroupsAsync(LanguageDTO language, int pageNo, int pageSize)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var countryGroups = await GetCountryGroupsDataAsync(localeLangId, pageNo, pageSize);

            // If localLanguage data is not available then pull the data based on default language
            if (countryGroups.CountryGroups.Count == 0)
            {
                countryGroups = await GetCountryGroupsDataAsync(dftLanguageId, pageNo, pageSize);
            }

            return countryGroups;
        }

        private async Task<CountryGroupResult> GetCountryGroupsDataAsync(int languageId, int pageNo, int pageSize)
        {
            var countryGroupsResult = new CountryGroupResult();

            // Get all countries 
            var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
                                    .Select(c => new { c.CountryId, c.DisplayName, c.DisplayNameShort })
                                    .AsNoTracking().ToListAsync();

            // Get all published countrygroups
            var countryGroups = await _dbContext.CountryGroups.Where(cg => cg.IsPublished.Equals(true) && cg.LanguageId.Equals(languageId))
                                           .Select(cgr => new { cgr.CountryGroupId, cgr.GroupName, cgr.AssociatedCountryIds })
                                           .OrderByDescending(cgr => cgr.CountryGroupId)
                                           .Skip((pageNo - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            if (countryGroups.Count == 0)
            {
                return null;
            }

            countryGroupsResult.CountryGroups.AddRange(countryGroups.Select(cg => new CountryGroupDTO
            {
                CountryGroupId = cg.CountryGroupId,
                GroupName = cg.GroupName,
                CountryGroupAssociatedCountries = countries.Where(c => cg.AssociatedCountryIds.Any(s => s.IdVal == c.CountryId))
                                                  .Select(co => new CountryDTO
                                                  {
                                                      Uuid = co.CountryId,
                                                      DisplayName = co.DisplayName,
                                                      DisplayNameShort = co.DisplayNameShort
                                                  }).ToList()
            }));
            return countryGroupsResult;
        }
    }
}
