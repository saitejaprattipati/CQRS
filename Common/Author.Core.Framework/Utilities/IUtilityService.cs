using Microsoft.AspNetCore.Http;

namespace Author.Core.Framework.Utilities
{
    public interface IUtilityService
    {
        bool IsTrusted();

        string GetLocale(IHeaderDictionary headersDictionary);
    }
}
