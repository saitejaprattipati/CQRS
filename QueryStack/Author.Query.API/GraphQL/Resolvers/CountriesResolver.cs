using Author.Core.Framework.Utilities;
using Author.Query.API.GraphQL.Types;
using Author.Query.Persistence.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Author.Query.API.GraphQL.Resolvers
{
    public class CountriesResolver : Resolver, ICountriesResolver
    {
        private readonly ICountryService _countryService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUtilityService _utilityService;

        public CountriesResolver(ICountryService countryService, IHttpContextAccessor accessor, IUtilityService utilityService)
        {
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _accessor = accessor;
            _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ResponseGraphType<CountryResultType>>(
                "countriesresponse",
                resolve: context =>
                {
                    var locale = _utilityService.GetLocale(_accessor.HttpContext.Request.Headers);
                    var list = _countryService.GetAllCountriesAsync(locale).GetAwaiter().GetResult();
                    return Response(list);
                }
                , description: "All Countries data"
            );
        }
    }
}
