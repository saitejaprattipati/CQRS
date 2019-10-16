using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain.DBAggregate
{
   public class Contacts
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int? ContactId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int? ImageId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public string OfficePhone { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string OfficeFaxNumber { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string MobilePhoneNumber { get; set; }

        /// <summary>gets or sets the Street</summary>
        /// <value>It is of type string </value>
        //[JsonProperty("street")]
        public string Email { get; set; }

        /// <summary>gets or sets the City </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("city")]
        public string Website { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public string FaceBookUrl { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public string TwitterUrl { get; set; }

        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public string LinkedInUrl { get; set; }

        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public string GooglePlusUrl { get; set; }

        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("stateEdited")]
        public string WeChatUrl { get; set; }

        /// <summary>gets or sets the CountryEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("countryEdited")]
        public string WeboUrl { get; set; }

        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string XINGUrl { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string VKUrl { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int? CountryId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public string CreatedBy { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string CreatedDate { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string UpdatedBy { get; set; }

        /// <summary>gets or sets the Street</summary>
        /// <value>It is of type string </value>
        //[JsonProperty("street")]
        public string UpdatedDate { get; set; }

        /// <summary>gets or sets the City </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("city")]
        public string EmpGUID { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public int? OfficePhoneEdited { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public int? MobilePhoneNumberEdited { get; set; }

        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public int? ImageIdEdited { get; set; }

        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public int? ContactContentId { get; set; }

        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("stateEdited")]
        public int? LanguageId { get; set; }

        /// <summary>gets or sets the CountryEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("countryEdited")]
        public string FirstName { get; set; }

        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string LastName { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string NativeName { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string Title { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public string Role { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string EmployeeLevel { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string Introduction { get; set; }

        /// <summary>gets or sets the Street</summary>
        /// <value>It is of type string </value>
        //[JsonProperty("street")]
        public string Organization { get; set; }

        /// <summary>gets or sets the City </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("city")]
        public string OrganizationUnitName { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public int? AddressId { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public int? FirstNameEdited { get; set; }

        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public int? LastNameEdited { get; set; }

        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public int? TitleEdited { get; set; }

        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("stateEdited")]
        public int? RoleEdited { get; set; }

        /// <summary>gets or sets the CountryEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("countryEdited")]
        public int? EmployeeLevelEdited { get; set; }
    }
    public partial class ContactsDetails
    {
        /// <summary>Gets or sets the Records in page.</summary>
        /// <value>It is of <see cref="IEnumerable{AddressAggregate}"/> Type </value>
        //[JsonProperty("records")]
        public IEnumerable<object> Records { get; set; }

        /// <summary>Gets or sets the Paging result</summary>
        /// <value>It is of <see cref="Author.Query.Domain.PagingResult"/> type</value>
        //[JsonProperty("pagingResult")]
        public PagingResult PagingResult { get; set; }
    }
}
