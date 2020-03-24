using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class AddressType:ObjectGraphType<AddressDTO>, IGraphQLType
    {
        public AddressType()
        {
            Name = "Address";
            Field(ad => ad.Street, nullable: true);
            Field(ad => ad.City, nullable: true);
            Field(ad => ad.Country, nullable: true);
        }
    }
}
