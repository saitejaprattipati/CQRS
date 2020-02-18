using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class CommonService : ICommonService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMapper _mapper;
        private readonly ICacheService<Languages, LanguageDTO> _cacheService;
        private readonly IHttpContextAccessor _accessor;

        public CommonService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings, IMapper mapper,
            ICacheService<Languages, LanguageDTO> cacheService,IHttpContextAccessor accessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appSettings = appSettings;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _accessor = accessor;
        }


        //public Languages GetLanguageFromLocale(string locale)
        public LanguageDTO GetLanguageFromLocale(string locale)
        {
            return GetLanguagesByFilter(GetFilterValues(locale, true).ToArray());
        }

        public async Task<LanguageDTO> GetLanguageFromLocaleAsync(string locale) =>
            await GetLanguagesByFilterAsync(GetFilterValues(locale, true).ToArray());

        //private Languages GetLanguagesByFilter(string[] filters)
        private async Task<LanguageDTO> GetLanguagesByFilterAsync(string[] filters)
        {
            try
            {
                if (filters.Length > 0)
                {
                    var languages = new List<Languages>();

                    //Languages[] languages;

                    ////var s = filters.First(); // requred by EF

                    // Get all Languages
                    var langData = await _cacheService.GetAllAsync("languagesCacheKey");

                    ////var query = _dbContext.Languages.Where(v => v.LocalisationIdentifier.Contains(s));
                    ////foreach (var item in filters.Where(v => v != filters[0]).Distinct())
                    ////{
                    ////    query = query.Union(_dbContext.Languages.Where(v => v.LocalisationIdentifier.Contains(item)));
                    ////}

                    ////languages = await query.ToListAsync();

                    //var languagesList = _mapper.Map<List<Languages>,List<LanguageDTO>>(languages);

                    foreach (var filter in filters)
                    {
                        //var lang = languages.FirstOrDefault(
                        //    l => l.LocalisationIdentifier
                        //        .Split(',')
                        //        .Any(i => i.Equals(filter, StringComparison.OrdinalIgnoreCase))
                        //);

                        //if (lang != null)
                        //{
                        //    lang.LocalisationIdentifier = lang.Locale ?? filter;
                        //    return lang;
                        //}

                        ////var lang = languages.FirstOrDefault(
                        ////        l => l.LocalisationIdentifier
                        ////            .Split(',')
                        ////            .Any(i => i.Equals(filter, StringComparison.OrdinalIgnoreCase))
                        ////    );

                        var lang = langData.FirstOrDefault(
                                l => l.LocalisationIdentifier
                                    .Split(',')
                                    .Any(i => i.Equals(filter, StringComparison.OrdinalIgnoreCase))
                            );

                        if (lang != null)
                        {
                            ////lang.LocalisationIdentifier = lang.Locale ?? filter;

                            ////var langDTO = _mapper.Map<LanguageDTO>(lang);

                            return lang;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private LanguageDTO GetLanguagesByFilter(string[] filters)
        {
            if (filters.Length > 0)
            {
                Languages[] languages;

                var s = filters.First(); // requred by EF

                var query = _dbContext.Languages.Where(v => v.LocalisationIdentifier.Contains(s));
                foreach (var item in filters.Where(v => v != filters[0]).Distinct())
                {
                    query = query.Union(_dbContext.Languages.Where(v => v.LocalisationIdentifier.Contains(item)));
                }

                languages = query.ToArray();

                var languagesList = _mapper.Map<LanguageDTO[]>(languages);

                foreach (var filter in filters)
                {
                    //var lang = languages.FirstOrDefault(
                    //    l => l.LocalisationIdentifier
                    //        .Split(',')
                    //        .Any(i => i.Equals(filter, StringComparison.OrdinalIgnoreCase))
                    //);

                    //if (lang != null)
                    //{
                    //    lang.LocalisationIdentifier = lang.Locale ?? filter;
                    //    return lang;
                    //}

                    var lang = languagesList.FirstOrDefault(
                            l => l.LocalisationIdentifier
                                .Split(',')
                                .Any(i => i.Equals(filter, StringComparison.OrdinalIgnoreCase))
                        );

                    if (lang != null)
                    {
                        lang.LocalisationIdentifier = lang.Locale ?? filter;
                        return lang;
                    }
                }
            }

            return null;
        }

        private IEnumerable<string> GetFilterValues(string locale, bool includeDefault)
        {
            //The LocalisationIdentifier may contain a comma separated list of locales.
            //Using Contains may return the wrong Language; e.g. "hi-IN" is incorrectly returned for "in" because it happens to come first

            var pattern = locale.Split(',')
                .Select(StringWithQualityHeaderValue.Parse)
                .Select(a => new StringWithQualityHeaderValue(a.Value, a.Quality.GetValueOrDefault(1)))
                .OrderByDescending(a => a.Quality)
                .Select(v => v.Value.ToLower());

            //return includeDefault ? pattern.Concat(new[] { ConfigurationManager.AppSettings["DefaultLocale"] }) : pattern;

            return includeDefault ? pattern.Concat(new[] { _appSettings.Value.DefaultLocale }) : pattern;
        }

        public LanguageDetailsDTO GetLanguageDetails()
        {
            var language = _accessor.HttpContext.Items["language"] as LanguageDTO;

            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            return new LanguageDetailsDTO { LocaleLangId = language.LanguageId , DefaultLanguageId = dftLanguageId};
        }
    }
}
