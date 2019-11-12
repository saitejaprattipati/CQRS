using Author.Core.Framework;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Author.Query.Persistence
{
    public class CountryService : ICountryService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly ICommonService _commonService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMapper _mapper;

        public CountryService(TaxathandDbContext dbContext, ICommonService commonService, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _appSettings = appSettings;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        //public async Task<CountryResult> GetAllCountries(string locale = "en-US")
        //public async Task<CountryResult> GetAllCountries(string locale)
        //{
        //    var result = new CountryResult();
        //    //var language = _commonService.GetLanguageFromLocale(locale);
        //    //var localeLangId = language.LanguageId;
        //    var localeLangId = 37;
        //    var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

        //    var countries = await (_dbContext.Countries.Where(cc => cc.IsPublished.Equals(true)
        //                                                     && cc.LanguageId.Equals(localeLangId))
        //                        .Join(_dbContext.Disclaimers.Where(dc => dc.LanguageId.Equals(localeLangId)), c => c.CountryId, d => d.DefaultCountryId, (c, d) =>
        //                             new
        //                             {
        //                                 c.CountryId,
        //                                 PngImageId = c.PNGImageId,
        //                                 SvgImageId = c.SVGImageId,
        //                                 DefaultDisplayName = _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId)).Select(df=>df.DisplayName).FirstOrDefault(),
        //                                 DisplayName = _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(localeLangId)).Select(df => df.DisplayName).FirstOrDefault(),
        //                                 d.ProviderName,
        //                                 d.ProviderTerms
        //                             }).ToListAsync());

        //    result.Countries.AddRange(countries.Select(country => new CountryDTO
        //    {
        //        DisplayName = country.DisplayName ?? country.DefaultDisplayName,
        //        DisplayNameShort = country.DisplayName ?? country.DefaultDisplayName,
        //        ProviderName = country.ProviderName,
        //        ProviderTerms = country.ProviderTerms,
        //        Uuid = country.CountryId,
        //        Name = Helper.ReplaceChars(country.DefaultDisplayName),
        //        Path = Helper.ReplaceChars(country.DefaultDisplayName),
        //        CompleteResponse = true
        //    }));
        //    return result;
        //}

        //public async Task<CountryResult> GetAllCountriesAsync(string locale)
        //{
        //    var result = new CountryResult();
        //    var language = _commonService.GetLanguageFromLocale(locale);
        //    var localeLangId = language.LanguageId;
        //    var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

        //    var dftDisplayName = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId)).Select(c => new { c.CountryId, c.DisplayName,c.DisplayNameShort }).ToListAsync();

        //    var countries = await (_dbContext.Countries.Where(cc => cc.IsPublished.Equals(true)
        //                                                     && cc.LanguageId.Equals(localeLangId))

        //                        .Join(_dbContext.Disclaimers.Where(dc => dc.LanguageId.Equals(localeLangId)), c => c.CountryId, d => d.DefaultCountryId, (c, d) =>
        //                        new CountryDTO
        //                            {
        //                                Uuid = c.CountryId,
        //                                DisplayName = c.DisplayName ?? dftDisplayName.Where(df => df.CountryId == c.CountryId).Select(x => x.DisplayNameShort).ToString(),
        //                                DisplayNameShort = c.DisplayName ?? dftDisplayName.Where(df => df.CountryId == c.CountryId).Select(x => x.DisplayNameShort).ToString(),
        //                                ProviderName = d.ProviderName,
        //                                ProviderTerms = d.ProviderTerms,
        //                                Name = dftDisplayName.Where(df => df.CountryId == c.CountryId).Select(x => x.DisplayNameShort).ToString(),
        //                                Path = dftDisplayName.Where(df => df.CountryId == c.CountryId).Select(x => x.DisplayNameShort).ToString(),
        //                                CompleteResponse = true
        //                            }).ToListAsync());

        //    result.Countries = countries;
        //    return result;
        //}

        public async Task<CountryResult> GetAllCountriesAsync(string locale)
        {
            int pageNo = 1, pageSize = 100;
            var result = new CountryResult();
            var language = _commonService.GetLanguageFromLocale(locale);
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            var countries = new List<CountryDTO>();
            if (dftLanguageId.Equals(localeLangId))
            {
                //countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId))
                //    .Select(dfc => new CountryDTO
                //    {
                //        Uuid = dfc.CountryId,
                //        DisplayName = dfc.DisplayName,
                //        DisplayNameShort = dfc.DisplayName,
                //        Name = Helper.ReplaceChars(dfc.DisplayName),
                //        Path = Helper.ReplaceChars(dfc.DisplayName),
                //        CompleteResponse = true
                //    }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();

                //var data = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId)).ToListAsync();

                countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId))
                    .Select(dfc => new CountryDTO
                    {
                        Uuid = dfc.CountryId,
                        DisplayName = dfc.DisplayName,
                        DisplayNameShort = dfc.DisplayName,
                        Name = Helper.ReplaceChars(dfc.DisplayName),
                        Path = Helper.ReplaceChars(dfc.DisplayName),
                        CompleteResponse = true
                    }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();
            }
            else
            {
                countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(localeLangId))
                                       .Join(
                                       _dbContext.Countries.Where(c => c.IsPublished.Equals(true)
                                                                  && c.LanguageId.Equals(dftLanguageId)),
                                        lc => lc.CountryId,
                                        dfc => dfc.CountryId,
                                        (lc, dfc) => new CountryDTO
                                        {
                                            Uuid = lc.CountryId,
                                            DisplayName = lc.DisplayName,
                                            DisplayNameShort = lc.DisplayName,
                                            Name = Helper.ReplaceChars(dfc.DisplayName),
                                            Path = Helper.ReplaceChars(dfc.DisplayName),
                                            CompleteResponse = true
                                        }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();
            }
            result.Countries = countries;
            return result;
        }

        public async Task<CountryResult> GetAllCountriesAsync(LanguageDTO language)
        {
            var result = new CountryResult();
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);
            
            result.Countries = await GetCountriesAsync(dftLanguageId, localeLangId);
            return result;
        }

        private async Task<List<CountryDTO>> GetCountriesAsync(int dftLanguageId, int localeLangId)
        {
            int pageNo = 1, pageSize = 100;
            var countries = new List<CountryDTO>();
            if (dftLanguageId.Equals(localeLangId))
            {
                //countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId))
                //    .Select(dfc => new CountryDTO
                //    {
                //        Uuid = dfc.CountryId,
                //        DisplayName = dfc.DisplayName,
                //        DisplayNameShort = dfc.DisplayName,
                //        Name = Helper.ReplaceChars(dfc.DisplayName),
                //        Path = Helper.ReplaceChars(dfc.DisplayName),
                //        CompleteResponse = true
                //    }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();

                countries = await _dbContext.Countries
                                      .Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(dftLanguageId))
                                      .ProjectTo<CountryDTO>(_mapper.ConfigurationProvider).ToListAsync();
            }
            else
            {
                countries = await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(localeLangId))
                                       .Join(
                                       _dbContext.Countries.Where(c => c.IsPublished.Equals(true)
                                                                  && c.LanguageId.Equals(dftLanguageId)),
                                        lc => lc.CountryId,
                                        dfc => dfc.CountryId,
                                        (lc, dfc) => new CountryDTO
                                        {
                                            Uuid = lc.CountryId,
                                            DisplayName = lc.DisplayName,
                                            DisplayNameShort = lc.DisplayName,
                                            Name = Helper.ReplaceChars(dfc.DisplayName),
                                            Path = Helper.ReplaceChars(dfc.DisplayName),
                                            CompleteResponse = true
                                        }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();
            }

            return countries;
        }
    }
}
