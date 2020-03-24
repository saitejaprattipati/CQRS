using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class RelatedArticleType : ObjectGraphType<RelatedArticleDTO>, IGraphQLType
    {
        public RelatedArticleType()
        {
            Name = "RelatedArticle";
            Field(ra => ra.ArticleId, nullable: true);
            Field(ra => ra.Title, nullable: true);
            Field<ListGraphType<CountryType>>(
                "relatedCountries",
                resolve: context =>
                {
                    return context.Source.RelatedCountries;
                }
            );
            Field(ra => ra.PublishedDate, nullable: true);
        }
    }

    public class RelatedResourceType : ObjectGraphType<RelatedArticleDTO>, IGraphQLType
    {
        public RelatedResourceType()
        {
            Name = "RelatedResource";
            Field(ra => ra.ArticleId, nullable: true);
            Field(ra => ra.Title, nullable: true);
            Field<ListGraphType<CountryType>>(
                "relatedCountries",
                resolve: context =>
                {
                    return context.Source.RelatedCountries;
                }
            );
            Field(ra => ra.PublishedDate, nullable: true);
        }
    }
}
