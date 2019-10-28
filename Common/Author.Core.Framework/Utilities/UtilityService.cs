using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;

namespace Author.Core.Framework.Utilities
{
    public class UtilityService : IUtilityService
    {
        private readonly IOptions<AppSettings> _appSettings;

        public string locale { get; set; }
        public UtilityService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public bool IsTrusted()
        {
            return _appSettings.Value.CustomErrorsMode.Trim().Equals(Convert.ToString(CustomErrorsMode.Off), StringComparison.OrdinalIgnoreCase);
        }

        public string GetLocale(IHeaderDictionary headersDictionary)
        {
            if (headersDictionary.ContainsKey(HeaderNames.AcceptLanguage))
            {
                locale = headersDictionary.GetCommaSeparatedValues(HeaderNames.AcceptLanguage)[0];
            }

            return locale;
        }
    }
}
