using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class DisclaimerResolver : Resolver, IDisclaimerResolver
    {
        private readonly IDisclaimerService _disclaimerService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;

        public DisclaimerResolver(IDisclaimerService disclaimerService, IHttpContextAccessor accessor,
                                IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _disclaimerService = disclaimerService ?? throw new ArgumentNullException(nameof(disclaimerService));
            _accessor = accessor;
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            var language = _accessor.HttpContext.Items["language"] as LanguageDTO;
            graphQLQuery.FieldAsync<ResponseGraphType<DisclaimerResultType>>(
                "disclaimersresponse",
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
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetAllDisclaimers",
                                                () => _disclaimerService.GetAllDisclaimersAsync(language, pageNo, pageSize));
                        var list = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(list);
                    }
                    return null;
                }
                , description: "All Disclaimers data"
            );

            graphQLQuery.FieldAsync<ResponseGraphType<DisclaimerType>>(
                "disclaimer",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "disclaimerId", Description = "id of the disclaimer" }
                ),
                resolve: async context =>
                {
                    var disclaimerId = context.GetArgument<int>("disclaimerId");
                    if (language != null && disclaimerId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetDisclaimer",
                                                () => _disclaimerService.GetDiscalimerAsync(language, disclaimerId));
                        var disclaimerDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(disclaimerDetails);
                    }

                    return null;
                }
            );
        }
    }
}
