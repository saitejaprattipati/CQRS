using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class DisclaimerType : ObjectGraphType<DisclaimerDTO>, IGraphQLType
    {
        public DisclaimerType()
        {
            Name = "Disclaimer";
            Field(x => x.DisclaimerId, nullable: true);
            Field(x => x.Name, nullable: true);
            Field(x => x.ProviderName, nullable: true);
            Field(x => x.ProviderTerms, nullable: true);
            Field(x => x.HomeCountry, nullable: true);
        }
    }

    public class DisclaimerResultType : ObjectGraphType<DisclaimerResult>
    {
        public DisclaimerResultType()
        {
            Field<ListGraphType<DisclaimerType>>(
                "disclaimers",
                resolve: context =>
                {
                    return context.Source.Disclaimers;
                }
            );
        }
    }
}
