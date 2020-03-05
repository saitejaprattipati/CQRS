using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class ArticleType : ObjectGraphType<ArticleDTO>, IGraphQLType
    {
        public ArticleType()
        {
            Name = "Article";
            Field(a => a.ArticleId, nullable: true);
            Field(a => a.Title, nullable: true);
            Field(a => a.TitleInEnglishDefault, nullable: true);
            Field(a => a.Author, nullable: true);
            Field<ListGraphType<ContactType>>(
                "relatedContacts",
                resolve: context =>
                {
                    return context.Source.RelatedContacts;
                }
            );
        }
    }
}
