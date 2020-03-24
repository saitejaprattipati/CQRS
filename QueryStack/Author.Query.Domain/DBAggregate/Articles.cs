using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain.DBAggregate
{
   public class Articles
    { 
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int ArticleId { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string PublishedDate { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public string Author { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public int ImageId { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string State { get; set; }

        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public int Type { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int SubType { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int ResourcePosition { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        //public int? DisclaimerId { get; set; }
        public DisclamersSchema Disclaimer { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public ResourceGroupsSchema ResourceGroup { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public bool IsPublished { get; set; }



        /// <summary>gets or sets the AddressId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("addressId")]
        public string CreatedBy { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public string CreatedDate { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public string UpdatedBy { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string UpdatedDate { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string NotificationSentDate { get; set; }

        public ProvinceSchema Province { get; set; }

        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("postCode")]
        public int ArticleContentId { get; set; }
        /// <summary>gets or sets the PostCodeEdited </summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("postCodeEdited")]
        public int LanguageId { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string Title { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string TitleInEnglishDefault { get; set; }

        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public string TeaserText { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public string Content { get; set; }
    
        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public ICollection<RelatedEntityId> RelatedContacts { get; set; }
        /// <summary>gets or sets the AddressContentId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("addressContentId")]
        public ICollection<RelatedEntityId> RelatedCountries { get; set; }

        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        public ICollection<RelatedEntityId> RelatedCountryGroups { get; set; }
        /// <summary>gets or sets LanguageId </summary>
        /// <value>It is of type integer </value>
        //[JsonProperty("languageId")]
        //public ICollection<RelatedEntityId> RelatedTaxTags { get; set; }
        public ICollection<RelatedTaxTagsSchema> RelatedTaxTags { get; set; }

        //public ICollection<RelatedEntityId> RelatedArticles { get; set; }
        public IEnumerable<RelatedArticlesSchema> RelatedArticles { get; set; }

        public IEnumerable<RelatedArticlesSchema> RelatedResources { get; set; }
    }   
    //public class RelatedContacts
    //{
    //    public string id { get; set; }
    //}
    //public class ArticleRelatedCountries
    //{
    //    public string id { get; set; }
    //}
    //public class ArticleRelatedCountryGroups
    //{
    //    public string id { get; set; }
    //}
    //public class ArticleRelatedTaxTags
    //{
    //    public string id { get; set; }
    //}
}
