using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.API.GraphQL.Types
{
    public class CountryType : ObjectGraphType<CountryDTO>, IGraphQLType
    {
        public CountryType()
        {
            Name = "Country";
            Field(x => x.PNGImagePath, nullable: true);
            Field(x => x.SVGImagePath, nullable: true);
            Field(x => x.DisplayName, nullable: true);
            Field(x => x.DisplayNameShort, nullable: true);
            Field(x => x.ProviderName, nullable: true);
            Field(x => x.ProviderTerms, nullable: true);
            Field(x => x.Uuid, nullable: true);
            Field(x => x.Name, nullable: true);
            Field(x => x.Path, nullable: true);
            Field(x => x.CompleteResponse);
        }
    }

    public class CountryResultType: ObjectGraphType<CountryResult>
    {
        public CountryResultType()
        {
            Field<ListGraphType<CountryType>>(
                "countries",
                resolve: context =>
                {
                    return context.Source.Countries;
                }
            );
        }
    }
}
