using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class CountryGroupType : ObjectGraphType<CountryGroupDTO>, IGraphQLType
    {
        public CountryGroupType()
        {
            Name = "CountryGroup";
            Field(cg => cg.CountryGroupId, nullable: true);
            Field(cg => cg.GroupName);
            Field<ListGraphType<CountryType>>(
                "countryGroupAssociatedCountries",
                resolve: context =>
                {
                    return context.Source.CountryGroupAssociatedCountries;
                }
            );
        }
    }

    public class CountryGroupResultType : ObjectGraphType<CountryGroupResult>
    {
        public CountryGroupResultType()
        {
            Field<ListGraphType<CountryGroupType>>(
                "countrygroups",
                resolve: context =>
                {
                    return context.Source.CountryGroups;
                }
            );
        }
    }
}
