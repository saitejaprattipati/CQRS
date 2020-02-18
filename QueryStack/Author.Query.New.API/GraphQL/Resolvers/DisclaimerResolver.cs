using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using System;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class DisclaimerResolver : Resolver, IDisclaimerResolver
    {
        private readonly IDisclaimerService _disclaimerService;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;

        public DisclaimerResolver(IDisclaimerService disclaimerService,
                                IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _disclaimerService = disclaimerService ?? throw new ArgumentNullException(nameof(disclaimerService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.FieldAsync<ResponseGraphType<DisclaimerResultType>>(
                "disclaimersresponse",
                resolve: async context =>
                {
                    var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetAllDisclaimers",
                                            () => _disclaimerService.GetAllDisclaimersAsync());
                    var list = await context.TryAsyncResolve(
                          async c => await loader.LoadAsync());
                    return Response(list);
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
                    if (disclaimerId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetDisclaimer",
                                                () => _disclaimerService.GetDiscalimerAsync(disclaimerId));
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
