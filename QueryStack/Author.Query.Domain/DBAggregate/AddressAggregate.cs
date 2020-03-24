using System.Collections.Generic;

namespace Author.Query.Domain.DBAggregate
{
    public class Address
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int AddressId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string PostCode { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public bool PostCodeEdited { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public int? AddressContentId { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public int? LanguageId { get; set; }

        /// <summary>gets or sets the Street</summary>
        /// <value>It is of type string </value>
        //[JsonProperty("street")]
        public string Street { get; set; }

        /// <summary>gets or sets the City </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("city")]
        public string City { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public string State { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public string Country { get; set; }
        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public bool StreetEdited { get; set; }
        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public bool CityEdited { get; set; }
        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("stateEdited")]
        public bool StateEdited { get; set; }
        /// <summary>gets or sets the CountryEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("countryEdited")]
        public bool CountryEdited { get; set; }
    }
    public partial class AddressAggregateDetails
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
