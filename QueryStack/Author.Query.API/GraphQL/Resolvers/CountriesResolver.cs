using Author.Core.Framework.Utilities;
using Author.Query.API.GraphQL.Types;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
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
            var language = _accessor.HttpContext.Items["language"] as LanguageDTO;
            graphQLQuery.FieldAsync<ResponseGraphType<CountryResultType>>(
                "countriesresponse",
                resolve: async context =>
                {
                    if (language != null)
                    {
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

            graphQLQuery.FieldAsync<ResponseGraphType<CountryType>>(
                "country",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "countryId", Description = "id of the country" }
                ),
                resolve: async context => {
                    var countryId = context.GetArgument<int>("countryId");
                    if (language != null && countryId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetCountry",
                                                () => _countryService.GetCountryAsync(language, countryId));
                        var countryDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(countryDetails);
                    }

                    return null;
                }
            );
        }
    }
}
