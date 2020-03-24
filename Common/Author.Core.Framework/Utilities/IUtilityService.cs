using Microsoft.AspNetCore.Http;

namespace Author.Core.Framework.Utilities
{
    public interface IUtilityService
    {
        bool IsTrusted();

        string GetLocale(IHeaderDictionary headersDictionary);

        string GetCookieId(HttpRequest request);

        string FormatArticleContent(string content);

        bool IsContainYouTubeLinks(string content);
    }
}
