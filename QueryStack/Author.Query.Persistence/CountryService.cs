using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
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
        private readonly ICacheService<Images, ImageDTO> _cacheService;

        public CountryService(TaxathandDbContext dbContext, ICommonService commonService, IImageService imageService, IOptions<AppSettings> appSettings, IMapper mapper,
            ICacheService<Images, ImageDTO> cacheService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
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

        public async Task<CountryDTO> GetCountryAsync(LanguageDTO language, int countryId)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            return await GetCountryDetailsAsync(countryId, dftLanguageId, localeLangId);
        }

        private async Task<CountryDTO> GetCountryDetailsAsync(int countryId, int dftLanguageId, int localeLangId)
        {
            var country = new CountryDTO();
            var images = await _cacheService.GetAllAsync("imagesCacheKey");
            if (dftLanguageId.Equals(localeLangId))
            {
                country = await _dbContext.Countries.Where(c => c.CountryId.Equals(countryId)
                                                                            && c.IsPublished.Equals(true)
                                                                            && c.LanguageId.Equals(dftLanguageId))
                                    .Select(dco => new CountryDTO
                                    {
                                        Uuid = dco.CountryId,
                                        PNGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(dco.PNGImageId)).FilePath,
                                        SVGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(dco.SVGImageId)).FilePath,
                                        DisplayName = dco.DisplayName,
                                        DisplayNameShort = dco.DisplayName,
                                        Name = Helper.ReplaceChars(dco.DisplayName),
                                        Path = Helper.ReplaceChars(dco.DisplayName),
                                        CompleteResponse = true
                                    }).FirstOrDefaultAsync();

            }
            else
            {
                country = await _dbContext.Countries.Where(cc => cc.CountryId.Equals(countryId) &&
                                                           cc.IsPublished.Equals(true) && cc.LanguageId.Equals(localeLangId))
                                      .Join(
                                      _dbContext.Countries.Where(c => c.CountryId.Equals(countryId) && c.IsPublished.Equals(true)
                                                                 && c.LanguageId.Equals(dftLanguageId)),
                                       lc => lc.CountryId,
                                       dfc => dfc.CountryId,
                                       (lc, dfc) => new CountryDTO
                                       {
                                           Uuid = lc.CountryId,
                                           PNGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(dfc.PNGImageId)).FilePath,
                                           SVGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(dfc.SVGImageId)).FilePath,
                                           DisplayName = lc.DisplayName,
                                           DisplayNameShort = lc.DisplayName,
                                           Name = Helper.ReplaceChars(dfc.DisplayName),
                                           Path = Helper.ReplaceChars(dfc.DisplayName),
                                           CompleteResponse = true
                                       }).FirstOrDefaultAsync();
            }

            return country;
        }

        private async Task<List<CountryDTO>> GetCountriesAsync(int dftLanguageId, int localeLangId)
        {
            var countries = new List<CountryDTO>();

            // By default pick the localLanguage value
            countries = await GetCountriesDataAsync(localeLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (countries.Count == 0)
            {
                countries = await GetCountriesDataAsync(dftLanguageId);
            }

            return countries;
        }
        private async Task<List<CountryDTO>> GetCountriesDataAsync(int languageId)
        {
            int pageNo = 1, pageSize = 100;
            var images = await _cacheService.GetAllAsync("imagesCacheKey");

            return await _dbContext.Countries.Where(cc => cc.IsPublished.Equals(true) && cc.LanguageId.Equals(languageId))
                    .Select(co => new CountryDTO
                    {
                        Uuid = co.CountryId,
                        PNGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(co.PNGImageId)).FilePath,
                        SVGImagePath = images.FirstOrDefault(im => im.ImageId.Equals(co.SVGImageId)).FilePath,
                        DisplayName = co.DisplayName,
                        DisplayNameShort = co.DisplayName,
                        Name = Helper.ReplaceChars(co.DisplayName),
                        Path = Helper.ReplaceChars(co.DisplayName),
                        CompleteResponse = true
                    }).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();
        }
    }
}
