using Author.Query.New.API.GraphQL.Types;
using Author.Query.Persistence.Interfaces;
using GraphQL.DataLoader;
using GraphQL.Types;
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

        public ArticlesResolver(IArticleService articleService,
                                IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _dataLoaderContextAccessor = dataLoaderContextAccessor;
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
                    if (articleId > 0 && countryId > 0)
                    {
                        var loader = _dataLoaderContextAccessor.Context.GetOrAddLoader("GetArticle",
                                                () => _articleService.GetArticleAsync(articleId,countryId));
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
