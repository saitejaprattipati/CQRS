namespace Author.Query.Persistence.DTO
{
    public class ContactDTO
    {
        public int ContactId { get; set; }

        public int? ImageId { get; set; }

        public string ImagePath { get; set; }

        public string OfficePhone { get; set; }

        public string OfficeFaxNumber { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string FaceBookUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string LinkedInUrl { get; set; }

        public string GooglePlusUrl { get; set; }

        public string WeChatUrl { get; set; }

        public string WeboUrl { get; set; }

        public string XINGUrl { get; set; }

        public string VKUrl { get; set; }

        public int? CountryId { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string EmpGUID { get; set; }

        public int? LanguageId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NativeName { get; set; }

        public string Title { get; set; }

        public string Role { get; set; }

        public string EmployeeLevel { get; set; }

        public string Introduction { get; set; }

        public string Organization { get; set; }

        public string OrganizationUnitName { get; set; }

        public int AddressId { get; set; }

        public string ContentType { get; set; }

        public AddressDTO address { get; set; }
    }
}
