using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class CountryGroupsResolver : Resolver, ICountryGroupsResolver
    {
        private readonly ICountryGroupService _countryGroupService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;

        public CountryGroupsResolver(ICountryGroupService countrygroupService, IHttpContextAccessor accessor,
                                     IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _countryGroupService = countrygroupService;
            _accessor = accessor;
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            var language = _accessor.HttpContext.Items["language"] as LanguageDTO;
            graphQLQuery.FieldAsync<ResponseGraphType<CountryGroupResultType>>(
                "countrygroupsresponse",
                arguments: new QueryArguments
                (
                    new QueryArgument<IdGraphType> { Name = "pageNo", Description = "page number" },
                    new QueryArgument<IdGraphType> { Name = "pageSize", Description = "page size" }
                ),
                resolve: async context =>
                {
                    var pageNo = context.GetArgument<int>("pageNo") == 0 ? 1 : context.GetArgument<int>("pageNo");
                    var pageSize = context.GetArgument<int>("pageSize") == 0 ? 100 : context.GetArgument<int>("pageSize");
                    if (language != null)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetCountryGroups",
                                                () => _countryGroupService.GetCountryGroupsAsync(language, pageNo, pageSize));
                        var list = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(list);
                    }
                    return null;
                }
                , description: "All CountryGroups data"
            );

            graphQLQuery.FieldAsync<ResponseGraphType<CountryGroupType>>(
                "countrygroup",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "countryGroupId", Description = "id of the countryGroup" }
                ),
                resolve: async context =>
                {
                    var countryGroupId = context.GetArgument<int>("countryGroupId");
                    if (language != null && countryGroupId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetCountryGroup",
                                                () => _countryGroupService.GetCountryGroupAsync(language, countryGroupId));
                        var countryGroupDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(countryGroupDetails);
                    }

                    return null;
                }
            );
        }
    }
}
