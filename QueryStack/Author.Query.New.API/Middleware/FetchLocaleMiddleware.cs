using Author.Core.Framework.Utilities;
using Author.Query.Persistence.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Author.Query.New.API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class FetchLocaleMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ICommonService _commonService;
        //private readonly IUtilityService _utilityService;

        public FetchLocaleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ICommonService commonService, IUtilityService utilityService)
        {
            var locale = utilityService.GetLocale(httpContext.Request.Headers);
            var language = await commonService.GetLanguageFromLocaleAsync(locale);

            httpContext.Items.Add("language", language);
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class FetchLocaleMiddlewareExtensions
    {
        public static IApplicationBuilder UseFetchLocaleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FetchLocaleMiddleware>();
        }
    }
}
