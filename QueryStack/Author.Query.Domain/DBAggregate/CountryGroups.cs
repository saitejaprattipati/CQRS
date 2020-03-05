using Newtonsoft.Json;
using System.Collections.Generic;


namespace Author.Query.Domain.DBAggregate
{
    public class CountryGroups
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int CountryGroupId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public bool IsPublished { get; set; }
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

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string UpdatedDate { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string CountryGroupContentId { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int LanguageId { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string GroupName { get; set; }

        public ICollection<RelatedEntityId> AssociatedCountryIds { get; set; }
    }

    public class RelatedEntityId
    {
        public int IdVal { get; set; }
    }

    public class RelatedArticlesSchema
    {
        [JsonProperty("ArticleId")]
        public int ArticleId { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("CountryId")]
        //public List<RelatedEntityId> CountryId { get; set; }

        public ICollection<RelatedEntityId> RelatedCountries { get; set; }

        [JsonProperty("PartitionKey")]
        public string PublishedDate { get; set; }

    }
    public class RelatedTaxTagsSchema
    {
        [JsonProperty("TaxTagId")]
        public int TaxTagId { get; set; }
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
    }

    public class ResourceGroupsSchema
    {
        [JsonProperty("ResourceGroupId")]
        public int ResourceGroupId { get; set; }
        [JsonProperty("Position")]
        public int Position { get; set; }
        [JsonProperty("GroupName")]
        public string GroupName { get; set; }
    }
    public class ProvinceSchema
    {
        [JsonProperty("ProvinceId")]
        public int ProvinceId { get; set; }
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
    }
    public class DisclamersSchema
    {
        [JsonProperty("DisclaimerId")]
        public int DisclaimerId { get; set; }
        [JsonProperty("ProviderName")]
        public string ProviderName { get; set; }
        [JsonProperty("ProviderTerms")]
        public string ProviderTerms { get; set; }
    }


}
