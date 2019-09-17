using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Author.Query.Domain
{
    public partial class AddressAggregate
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
        /// <value>It is of type string </value>
        //[JsonProperty("postCodeEdited")]
        public string PostCodeEdited { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("addressContentId")]
        public string AddressContentId { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public int LanguageId { get; set; }

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
        /// <value>It is of type string </value>
        //[JsonProperty("streetEdited")]
        public string StreetEdited { get; set; }
        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("cityEdited")]
        public string CityEdited { get; set; }
        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("stateEdited")]
        public string StateEdited { get; set; }
        /// <summary>gets or sets the CountryEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("countryEdited")]
        public string CountryEdited { get; set; }

    }
    public partial class AddressAggregateDetails
    {
        /// <summary>Gets or sets the Records in page.</summary>
        /// <value>It is of <see cref="IEnumerable{WorkOfferAggregate}"/> Type </value>
        //[JsonProperty("records")]
        public IEnumerable<AddressAggregate> Records { get; set; }

        /// <summary>Gets or sets the Paging result</summary>
        /// <value>It is of <see cref="OrderFulfillment.Query.Domain.PagingResult"/> type</value>
        //[JsonProperty("pagingResult")]
        public PagingResult PagingResult { get; set; }
    }
    public partial class AddressAggregate
    {
        public static AddressAggregate FromJson(string json) => JsonConvert.DeserializeObject<AddressAggregate>(json);
    }
}
