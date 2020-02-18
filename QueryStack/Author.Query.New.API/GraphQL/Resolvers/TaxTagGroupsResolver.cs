using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class TaxTagGroupsResolver : Resolver, ITaxTagGroupsResolver
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;
        private readonly ITaxTagsService _taxTagsService;

        public TaxTagGroupsResolver(ITaxTagsService taxTagsService, IHttpContextAccessor accessor,
                                     IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _taxTagsService = taxTagsService ?? throw new ArgumentNullException(nameof(taxTagsService));
            _accessor = accessor;
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            var language = _accessor.HttpContext.Items["language"] as LanguageDTO;
            graphQLQuery.FieldAsync<ResponseGraphType<TaxTagGroupResultType>>(
                "taxtaggroupsresponse",
                resolve: async context =>
                {
                    if (language != null)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetTaxTagGroups",
                                                () => _taxTagsService.GetTaxTagGroupsAsync(language));
                        var list = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(list);
                    }
                    return null;
                }
                , description: "All TaxTagGroups data"
            );

            graphQLQuery.FieldAsync<ResponseGraphType<TaxTagGroupType>>(
                "taxtaggroup",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "taxTagId", Description = "id of the taxTagGroup" }
                ),
                resolve: async context =>
                {
                    var taxTagId = context.GetArgument<int>("taxTagId");
                    if (language != null && taxTagId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetTaxTagGroup",
                                                () => _taxTagsService.GetTaxTagGroupAsync(language, taxTagId));
                        var taxTagGroupDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(taxTagGroupDetails);
                    }

                    return null;
                }
            );
        }
    }
}
