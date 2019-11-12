using Author.Core.Framework.Utilities;
using Author.Query.API.GraphQL.Types;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using Microsoft.AspNetCore.Http;
using System;

namespace Author.Query.API.GraphQL.Resolvers
{
    public class CountriesResolver : Resolver, ICountriesResolver
    {
        private readonly ICountryService _countryService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUtilityService _utilityService;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;

        public CountriesResolver(ICountryService countryService, IHttpContextAccessor accessor, IUtilityService utilityService,
                                IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _accessor = accessor;
            _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.FieldAsync<ResponseGraphType<CountryResultType>>(
                "countriesresponse",
                resolve: async context =>
                {
                    //var locale = _utilityService.GetLocale(_accessor.HttpContext.Request.Headers);
                    var language = _accessor.HttpContext.Items["language"] as LanguageDTO;
                    if (language != null)
                    {
                        //var list = _countryService.GetAllCountriesAsync(locale).GetAwaiter().GetResult();
                        ////var list = _countryService.GetAllCountriesAsync(language).GetAwaiter().GetResult();

                        ////var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetAllCountries",
                        ////                                        () => _countryService.GetAllCountriesAsync(language));
                        ////var list = loader.LoadAsync();
                        ////return Response(list);

                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetAllCountries",
                                                () => _countryService.GetAllCountriesAsync(language));
                        var list = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(list);
                    }
                    return null;
                }
                , description: "All Countries data"
            );
        }
    }
}
