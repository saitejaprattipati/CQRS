using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class ContactType : ObjectGraphType<ContactDTO>, IGraphQLType
    {
        public ContactType()
        {
            Name = "Contact";
            Field(co => co.ContactId, nullable: true);
            Field(co => co.OfficePhone, nullable: true);
            Field(co => co.OfficeFaxNumber, nullable: true);
            Field(co => co.MobilePhoneNumber, nullable: true);
            Field(co => co.Email, nullable: true);
            Field(co => co.Website, nullable: true);
            Field(co => co.FaceBookUrl, nullable: true);
            Field(co => co.TwitterUrl, nullable: true);
            Field(co => co.LinkedInUrl, nullable: true);
            Field(co => co.GooglePlusUrl, nullable: true);
            Field(co => co.WeChatUrl, nullable: true);
            Field(co => co.WeboUrl, nullable: true);
            Field(co => co.XINGUrl, nullable: true);
            Field(co => co.VKUrl, nullable: true);
            Field(co => co.FirstName, nullable: true);
            Field(co => co.LastName, nullable: true);
            Field(co => co.NativeName, nullable: true);
            Field(co => co.Title, nullable: true);
            Field(co => co.Role, nullable: true);
            Field(co => co.EmployeeLevel, nullable: true);
            Field(co => co.Introduction, nullable: true);
            Field(co => co.Organization, nullable: true);
            Field(co => co.OrganizationUnitName, nullable: true);
            Field(co => co.ImagePath, nullable: true);
            Field(co => co.ContentType, nullable: true);
            Field<AddressType>(
                "address",
                resolve: context =>
                {
                    return context.Source.address;
                }
            );
        }
    }
}
