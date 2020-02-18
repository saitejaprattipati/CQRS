using Author.Core.Framework.Utilities;
using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class CountriesResolver : Resolver, ICountriesResolver
    {
        private readonly ICountryService _countryService;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;

        public CountriesResolver(ICountryService countryService,
                                IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.FieldAsync<ResponseGraphType<CountryResultType>>(
                "countriesresponse",
                arguments: new QueryArguments
                (
                    new QueryArgument<IdGraphType> { Name = "pageNo", Description = "page number" },
                    new QueryArgument<IdGraphType> { Name = "pageSize", Description = "page size" }
                ),
                resolve: async context =>
                {
                    var pageNo = context.GetArgument<int>("pageNo") == 0 ? 1 : context.GetArgument<int>("pageNo");
                    var pageSize = context.GetArgument<int>("pageSize") == 0 ? 10000 : context.GetArgument<int>("pageSize");

                    var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetAllCountries",
                                            () => _countryService.GetAllCountriesAsync(pageNo, pageSize));
                    var list = await context.TryAsyncResolve(
                          async c => await loader.LoadAsync());
                    return Response(list);
                }
                , description: "All Countries data"
            );

            graphQLQuery.FieldAsync<ResponseGraphType<CountryType>>(
                "country",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "countryId", Description = "id of the country" }
                ),
                resolve: async context =>
                {
                    var countryId = context.GetArgument<int>("countryId");
                    if (countryId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetCountry",
                                                () => _countryService.GetCountryAsync(countryId));
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
