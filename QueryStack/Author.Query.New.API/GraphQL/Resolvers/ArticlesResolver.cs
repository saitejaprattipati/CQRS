using Author.Core.Framework.Utilities;
using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class ArticlesResolver : Resolver, IArticlesResolver
    {
        private readonly IArticleService _articleService;
        private readonly IDataLoaderContextAccessor _dataLoaderContextAccessor;
        private readonly IUtilityService _utilityService;
        private readonly IHttpContextAccessor _accessor;

        public ArticlesResolver(IArticleService articleService,
                                IDataLoaderContextAccessor dataLoaderContextAccessor, IUtilityService utilityService,
                                IHttpContextAccessor accessor)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
            _accessor = accessor;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.FieldAsync<ResponseGraphType<ArticleType>>(
                "article",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "articleId", Description = "id of the article" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "countryId", Description = "id of the country " }
                    ),
                resolve: async context =>
                {
                    var articleId = context.GetArgument<int>("articleId");
                    var countryId = context.GetArgument<int>("countryId");
                    var userCookieId = _utilityService.GetCookieId(_accessor.HttpContext.Request);

                    if (articleId > 0 && countryId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetArticle",
                                                () => _articleService.GetArticleAsync(articleId,countryId, userCookieId));
                        var articleDetails = await context.TryAsyncResolve(
                              async c => await loader.LoadAsync());
                        return Response(articleDetails);
                    }

                    return null;
                }
            );
        }
    }
}
