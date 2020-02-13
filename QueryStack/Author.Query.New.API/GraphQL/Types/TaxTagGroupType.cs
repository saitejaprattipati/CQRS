using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class TaxTagGroupType : ObjectGraphType<TaxTagGroupDTO>, IGraphQLType
    {
        public TaxTagGroupType()
        {
            Name = "TaxTagGroup";
            Field(tg => tg.TaxTagId, nullable: true);
            Field(tg => tg.DisplayName, nullable: true);
            Field<ListGraphType<TaxTagsType>>(
                "associatedTags",
                resolve: context =>
                {
                    return context.Source.AssociatedTags;
                }
            );
        }
    }

    public class TaxTagsType : ObjectGraphType<TaxTagsDTO>
    {
        public TaxTagsType()
        {
            Name = "TaxTag";
            Field(tt => tt.TaxTagId, nullable: true);
            Field(tt => tt.ParentTagId, nullable: true);
            Field(tt => tt.DisplayName, nullable: true);
            Field<ListGraphType<CountryType>>(
                "relatedCountries",
                resolve: context =>
                {
                    return context.Source.RelatedCountries;
                }
            );
        }
    }


    public class TaxTagGroupResultType : ObjectGraphType<TaxTagGroupsResult>
    {
        public TaxTagGroupResultType()
        {
            Field<ListGraphType<TaxTagGroupType>>(
                "taxtaggroups",
                resolve: context =>
                {
                    return context.Source.TaxTagGroups;
                }
            );
        }
    }
}
