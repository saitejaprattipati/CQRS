using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain.DBAggregate
{
   public class Disclaimers
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int? DisclaimerId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string Name { get; set; }
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
        public int? DefaultCountryId { get; set; }

        /// <summary>gets or sets the State</summary>
        /// <value>It is of type string</value>
        //[JsonProperty("state")]
        public int? DisclaimerContentId { get; set; }

        /// <summary>gets or sets the Country </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("country")]
        public int? LanguageId { get; set; }
        /// <summary>gets or sets the StreetEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("streetEdited")]
        public string ProviderName { get; set; }
        /// <summary>gets or sets the CityEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("cityEdited")]
        public string ProviderTerms { get; set; }
    }
}
