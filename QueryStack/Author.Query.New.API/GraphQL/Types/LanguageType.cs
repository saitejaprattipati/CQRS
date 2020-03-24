using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class LanguageType: ObjectGraphType<LanguageDTO>, IGraphQLType
    {
        public LanguageType()
        {
            Name = "Language";
            Field(lg => lg.LanguageId, nullable: true);
            Field(lg => lg.Name, nullable: true);
            Field(lg => lg.NameinEnglish, nullable: true);
            Field(lg => lg.LocalisationIdentifier, nullable: true);
            Field(lg => lg.Locale, nullable: true);
            Field(lg => lg.RightToLeft, nullable: true);
        }
    }
}
