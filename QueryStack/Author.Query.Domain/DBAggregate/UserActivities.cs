using System.Collections.Generic;

namespace Author.Query.Domain.DBAggregate
{
    public class UserActivities
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int WebsiteUserId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public List<UserReadArticles> ReadArticles { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public List<UserSavedArticles> SavedArticles { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public List<UserSubscribedCountries> SubscribedCountries { get; set; }
    }
    public class UserSavedArticles
    {
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
       //[JsonProperty("postCode")]
        public int UserSavedArticleId { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int ArticleId { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string CreatedBy { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string CreatedDate { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string UpdatedBy { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string UpdatedDate { get; set; }
    }
    public class UserReadArticles
    {

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int? UserReadArticleId { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int? ArticleId { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string CreatedBy { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string CreatedDate { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string UpdatedBy { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string UpdatedDate { get; set; }
    }
    public class UserSubscribedCountries
    {
        public int UserSubscribedCountryId { get; set; }
        public IEnumerable<SubscribedCountryTags> Country { get; set; }
    }
    public class SubscribedCountryTags
    {
        public int CountryId { get; set; }
        public int TaxTagId { get; set; }
    }
}
