using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain.DBAggregate
{
   public class Countries
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int CountryId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int? SVGImageId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int? PNGImageId { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public bool IsPublished { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string CreatedBy { get; set; }

        /// <summary>gets or sets the Street</summary>
        /// <value>It is of type string </value>
        //[JsonProperty("street")]
        public string CreatedDate { get; set; }

        /// <summary>gets or sets the City </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("city")]
        public string UpdatedBy { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public string UpdatedDate { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public int? CountryContentId { get; set; }

        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public int? LanguageId { get; set; }

        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public string DisplayName { get; set; }

        /// <summary>gets or sets the StateEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("stateEdited")]
        public string DisplayNameShort { get; set; }
    }
}
