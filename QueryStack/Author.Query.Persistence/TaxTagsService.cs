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
    public class TaxTagsService : ITaxTagsService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;

        public TaxTagsService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appSettings = appSettings;
        }

        public async Task<TaxTagGroupsResult> GetTaxTagGroupsAsync(LanguageDTO language, int pageNo, int pageSize)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var taxTagGroups = await GetAllTaxTagGroupsAsync(localeLangId, pageNo, pageSize);

            // If localLanguage data is not available then pull the data based on default language
            if (taxTagGroups.TaxTagGroups.Count == 0)
            {
                taxTagGroups = await GetAllTaxTagGroupsAsync(dftLanguageId, pageNo, pageSize);
            }

            return taxTagGroups;
        }

        private async Task<TaxTagGroupsResult> GetAllTaxTagGroupsAsync(int languageId, int pageNo, int pageSize)
        {
            var taxTagGroupsResult = new TaxTagGroupsResult();

            // Get all countries 
            var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
                                    .Select(c => new { c.CountryId, c.DisplayName, c.DisplayNameShort })
                                    .AsNoTracking().ToListAsync();

            // Get all published taxTags            
            var taxTags = await _dbContext.TaxTags.Where(tg => tg.IsPublished.Equals(true) && tg.LanguageId.Equals(languageId))
                                                      .Select(tg => new { tg.TaxTagId, tg.ParentTagId, tg.DisplayName, tg.RelatedCountryIds })
                                                      .OrderByDescending(tg => tg.TaxTagId)
                                                      .AsNoTracking().ToListAsync();


            if (taxTags.Count == 0 || taxTags == null)
            {
                return null;
            }

            var parentTaxTagGroups = taxTags.Where(tg => tg.ParentTagId == null);

            taxTagGroupsResult.TaxTagGroups.AddRange(parentTaxTagGroups.Select(tgrp => new TaxTagGroupDTO
            {
                TaxTagId = tgrp.TaxTagId,
                ParentTagId = tgrp.ParentTagId,
                DisplayName = tgrp.DisplayName,
                AssociatedTags = taxTags.Where(tg => tg.ParentTagId.Equals(tgrp.TaxTagId)).Select(tt => new TaxTagsDTO
                {
                    TaxTagId = tt.TaxTagId,
                    DisplayName = tt.DisplayName,
                    RelatedCountries = countries.Where(c => tt.RelatedCountryIds.Any(s => s.IdVal.Equals(c.CountryId)))
                                                   .Select(co => new CountryDTO
                                                   {
                                                       Uuid = co.CountryId,
                                                       DisplayName = co.DisplayName,
                                                       DisplayNameShort = co.DisplayNameShort
                                                   }).ToList()

                }).ToList()
            }));

            return taxTagGroupsResult;
        }

        public async Task<TaxTagGroupDTO> GetTaxTagGroupAsync(LanguageDTO language, int taxTagId)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var taxTagGroup = await GetTaxTagGroupDataAsync(taxTagId, localeLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (taxTagGroup == null)
            {
                taxTagGroup = await GetTaxTagGroupDataAsync(taxTagId, dftLanguageId);
            }

            return taxTagGroup;
        }

        private async Task<TaxTagGroupDTO> GetTaxTagGroupDataAsync(int taxTagId, int languageId)
        {
            // Get all countries 
            var countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
                                    .Select(c => new { c.CountryId, c.DisplayName, c.DisplayNameShort })
                                    .AsNoTracking().ToListAsync();


            // Get all published taxTags            
            var taxTags = await _dbContext.TaxTags.Where(tg => tg.IsPublished.Equals(true) && tg.LanguageId.Equals(languageId))
                                                       .Select(tg => new { tg.TaxTagId, tg.ParentTagId, tg.DisplayName, tg.RelatedCountryIds })
                                                       .OrderByDescending(tg => tg.TaxTagId)
                                                       .AsNoTracking().ToListAsync();

            var taxTag = taxTags.Where(tg => tg.TaxTagId.Equals(taxTagId) && tg.ParentTagId == null).FirstOrDefault();

            if (taxTag == null)
            {
                return null;
            }

            var taxTagGroup = new TaxTagGroupDTO
            {
                TaxTagId = taxTag.TaxTagId,
                DisplayName = taxTag.DisplayName,
                AssociatedTags = taxTags.Where(tg => tg.ParentTagId.Equals(taxTagId)).Select(tt => new TaxTagsDTO
                {
                    TaxTagId = tt.TaxTagId,
                    DisplayName = tt.DisplayName,
                    RelatedCountries = countries.Where(c => tt.RelatedCountryIds.Any(s => s.IdVal.Equals(c.CountryId)))
                                                   .Select(co => new CountryDTO
                                                   {
                                                       Uuid = co.CountryId,
                                                       DisplayName = co.DisplayName,
                                                       DisplayNameShort = co.DisplayNameShort
                                                   }).ToList()

                }).ToList()
            };

            return taxTagGroup;
        }
    }
}
