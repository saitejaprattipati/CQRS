using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using System;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class ResourceGroupResolver : Resolver, IResourceGroupResolver
    {
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;
        private readonly IResourceGroupService _resourceGroupService;


        public ResourceGroupResolver(IResourceGroupService resourceGroupService,
            IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.FieldAsync<ResponseGraphType<ResourceGroupResultType>>(
                "resourcegroupsresponse",
                arguments: new QueryArguments
                (
                    new QueryArgument<IdGraphType> { Name = "pageNo", Description = "page number" },
                    new QueryArgument<IdGraphType> { Name = "pageSize", Description = "page size" }
                ),
                resolve: async context =>
                {
                    var pageNo = context.GetArgument<int>("pageNo") == 0 ? 1 : context.GetArgument<int>("pageNo");
                    var pageSize = context.GetArgument<int>("pageSize") == 0 ? 10000 : context.GetArgument<int>("pageSize");

                    var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetResourceGroups",
                                            () => _resourceGroupService.GetResourceGroupsAsync(pageNo, pageSize));
                    var list = await context.TryAsyncResolve(
                          async c => await loader.LoadAsync());
                    return Response(list);

                }
                , description: "All ResourceGroups data"
            );

            graphQLQuery.FieldAsync<ResponseGraphType<ResourceGroupType>>(
                "resourcegroup",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "resourceGroupId", Description = "id of the resourcegroup" }
                ),
                resolve: async context =>
                {
                    var resourceGroupId = context.GetArgument<int>("resourceGroupId");
                    if (resourceGroupId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetResourceGroup",
                                                () => _resourceGroupService.GetResourceGroupAsync(resourceGroupId));
                        var resourceGroupDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(resourceGroupDetails);
                    }

                    return null;
                }
            );
        }
    }
}

