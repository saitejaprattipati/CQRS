using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Author.Query.Persistence
{
    public class CommonService : ICommonService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        public CommonService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext ??  throw new ArgumentNullException(nameof(dbContext));
            _appSettings = appSettings;
        }


        public Languages GetLanguageFromLocale(string locale)
        {
            return GetLanguagesByFilter(GetFilterValues(locale, true).ToArray());
        }

        private Languages GetLanguagesByFilter(string[] filters)
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

                foreach (var filter in filters)
                {
                    var lang = languages.FirstOrDefault(
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
    }
}
