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
            ICacheService<Languages, LanguageDTO> cacheService, IHttpContextAccessor accessor)
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

            return new LanguageDetailsDTO { LocaleLangId = language.LanguageId, DefaultLanguageId = dftLanguageId };
        }

        public async Task<LanguageDTO> GetDefaultLanguageAsync()
        {
            var defaultLocalizationIdentifier = _appSettings.Value.DefaultLanguageId;

            return await SelectLanguageAsync(defaultLocalizationIdentifier);
        }

        private async Task<LanguageDTO> SelectLanguageAsync(string lang)
        {
            //The LocalisationIdentifier may contain a comma separated list of locales.
            //Using Contains may return the wrong Language; e.g. "hi-IN" is incorrectly returned for "in" because it happens to come first
            var langData = await _cacheService.GetAllAsync("languagesCacheKey");

            var language = langData.Where(l => l.LocalisationIdentifier.Contains(lang))
                                   .ToList()
                                    //Then split identifier for final check and get first (will be done on objects, so explicitly ignore case)
                                    .FirstOrDefault(l => l.LocalisationIdentifier.Split(',').Any(i => i.Equals(lang, StringComparison.OrdinalIgnoreCase)));

            if (language != null)
            {
                language.LocalisationIdentifier = language.Locale ?? lang;
            }

            return language;
        }

        public async Task<List<LanguageDTO>> GetLanguageListFromLocale(string locale)
        {
            var languages = Enumerable.Empty<StringWithQualityHeaderValue>().OrderBy(s => s);

            if (!string.IsNullOrWhiteSpace(locale))
            {
                languages = locale.Split(new[] { ',' })
                    .Select(a => StringWithQualityHeaderValue.Parse(a))
                    .Select(a => new StringWithQualityHeaderValue(a.Value,
                        a.Quality.GetValueOrDefault(1)))
                    .OrderByDescending(a => a.Quality);
            }

            var languageDict = new Dictionary<int, LanguageDTO>();
            LanguageDTO language = null;

            //This will return there primary language first as per the locale 
            //Need special handling for simplied and traditional chinese.  Both start with zh so we need additional code
            //to handle the case
            bool isChineseTraditional = false;
            bool isChineseSimplified = false;
            int preferredLanguageId = 0;
            string preferredLanguage = "";
            foreach (var lang in languages)
            {
                language = await SelectLanguageAsync(lang.Value);
                if (language != null)
                {
                    preferredLanguageId = language.LanguageId;
                    preferredLanguage = language.Locale.Split('-')[0];
                    languageDict.Add(language.LanguageId, language);

                    if (language.Locale.ToLower() == "zh-tw"
                        || language.Locale.ToLower() == "zh-hk")
                    {
                        isChineseTraditional = true;
                    }
                    else if (language.Locale.ToLower() == "zh-cn"
                             || language.Locale.ToLower() == "zh-sg")
                    {
                        isChineseSimplified = true;
                    }
                }
            }

            //See if we can grab any other languages that would be useful to display the article in if the
            //primary language is not available
            //It just needs to look over all languages in the database and see if we find any thing that we consider
            //a match - ie the first 2 digits of the local match
            var languageListServer = await _cacheService.GetAllAsync("languagesCacheKey");

            //Need special handling for chinese simplifed and traditional
            if (isChineseTraditional)
            {
                //Add remaining chinese traditional languages
                foreach (var item in languageListServer)
                {
                    if (preferredLanguageId != item.LanguageId &&
                        (item.Locale.ToLower() == "zh-tw" || item.Locale.ToLower() == "zh-hk"))
                    {
                        languageDict.Add(item.LanguageId, item);
                    }
                }
            }
            else if (isChineseSimplified)
            {
                //Add remaining chinese simplifed languages
                foreach (var item in languageListServer)
                {
                    if (preferredLanguageId != item.LanguageId &&
                        (item.Locale.ToLower() == "zh-cn" || item.Locale.ToLower() == "zh-sg"))
                    {
                        languageDict.Add(item.LanguageId, item);
                    }
                }
            }

            //Add any remaining languages to the list.  This will match on the first part of the locale string ie es or zh
            foreach (var item in languageListServer)
            {
                if (item.Locale.Split('-')[0] == preferredLanguage && !languageDict.ContainsKey(item.LanguageId))
                {
                    languageDict.Add(item.LanguageId, item);
                }
            }

            //Didn't get a language so just grab the default one
            if (languageDict.Count == 0)
            {
                var defaultLocalizationIdentifier = _appSettings.Value.DefaultLanguageId;
                var defaultLanguage = await SelectLanguageAsync(defaultLocalizationIdentifier);
                languageDict.Add(defaultLanguage.LanguageId, defaultLanguage);
            }

            var languageList = languageDict.Values.ToList();
            return languageList;
        }

        public long GetUnixEpochTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)date.Subtract(epoch).TotalMilliseconds;
        }

        public DateTime GetDateTimeFromUnixEpochTime(long value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(value);
        }
    }
}
