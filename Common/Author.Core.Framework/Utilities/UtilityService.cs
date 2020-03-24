using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Author.Core.Framework.Utilities
{
    public class UtilityService : IUtilityService
    {
        private const string UriRegexPattern =
            @"(https?|ftp)://(-\.)?([^\s/?\.#-]+\.?)+(/[^\s]*)?$";
        private const string Iframe = "<iframe.+?src=[\"'](.+?)[\"'].*?>";

        private static readonly Regex UriRegex = new Regex(UriRegexPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex IframeRegex = new Regex(Iframe, RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

        public string GetCookieId(HttpRequest request)
        {
            string cookieId = string.Empty;

            var cookies = request.Cookies;
            if (cookies.Any())
            {
                cookies.TryGetValue(Constants.TAX_COOKIE_BROWSER_NAME, out string cookiedata);
                cookieId = (cookiedata != null) ? cookiedata.ToLower() : cookieId;
            }

            return cookieId;
        }

        public string FormatArticleContent(string content)
        {
            //var baseUri = new Uri(publicSiteUrl);
            var baseUri = new Uri(_appSettings.Value.PublicSiteBaseUrl);
            var tags = new[] { "img", "iframe", "video", "audio" };

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            foreach (var node in doc.DocumentNode.Descendants().Where(n => tags.Contains(n.Name)))
            {
                if (node.HasAttributes)
                {
                    var attr = node.Attributes["src"];
                    if (attr != null)
                    {
                        attr.Value = GetAbsoluteUrl(baseUri, attr.Value);
                    }
                }
            }

            using (var writer = new StringWriter())
            {
                doc.Save(writer);
                var formattedContent = writer.ToString();
                return formattedContent;
            }
        }
        private string GetAbsoluteUrl(Uri baseUri, string srcUrl)
        {
            var uri = new Uri(baseUri, srcUrl);
            var hadDefaultPort = uri.IsDefaultPort;
            var builder = new UriBuilder(uri);
            builder.Scheme = baseUri.Scheme;
            if (!builder.Host.Equals(baseUri.Host, StringComparison.OrdinalIgnoreCase) && hadDefaultPort)
            {
                builder.Port = -1;
            }
            return builder.Uri.AbsoluteUri;
        }

        public bool IsContainYouTubeLinks(string content)
        {
            var result = false;
            var baseUri = new Uri(_appSettings.Value.PublicSiteBaseUrl);
            var matches = IframeRegex.Matches(content);

            foreach (Match match in matches)
            {
                var m = match.Groups[1].Value;

                var s = GetAbsoluteUrl(baseUri,m);

                if (UriRegex.IsMatch(s))
                {
                    var uri = new Uri(s);
                    if (uri.Host.ToLower().Contains(Constants.YouTube_Link))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
