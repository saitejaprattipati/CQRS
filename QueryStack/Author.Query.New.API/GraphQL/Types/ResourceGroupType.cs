using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class ResourceGroupType : ObjectGraphType<ResourceGroupDTO>, IGraphQLType
    {
        public ResourceGroupType()
        {
            Name = "ResourceGroup";
            Field(rg => rg.ResourceGroupId);
            Field(rg => rg.Position, nullable: true);
            Field(rg => rg.GroupName, nullable: true);
        }
    }

    public class ResourceGroupResultType : ObjectGraphType<ResourceGroupResult>
    {
        public ResourceGroupResultType()
        {
            Field<ListGraphType<ResourceGroupType>>(
                "resourcegroups",
                resolve: context =>
                {
                    return context.Source.ResourceGroups;
                }
            );
        }
    }
}
