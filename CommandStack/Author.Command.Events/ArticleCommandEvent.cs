using Author.Command.Domain.Models;
using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Author.Command.Events
{
   public class ArticleCommandEvent : IntegrationEvent
    {
        [JsonProperty("ArticleId")]
        public int ArticleId { get; set; }

        [JsonProperty("PublishedDate")]
        public string PublishedDate { get; set; }

        [JsonProperty("Author")]
        public string Author { get; set; }

        [JsonProperty("ImageId")]
        public int? ImageId { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Type")]
        public int Type { get; set; }

        [JsonProperty("SubType")]
        public int? SubType { get; set; }

        [JsonProperty("ResourcePosition")]
        public int? ResourcePosition { get; set; }

        [JsonProperty("Disclaimer")]
        public DisclamersSchema Disclaimer { get; set; }

        [JsonProperty("ResourceGroup")]
        public ResourceGroupsSchema ResourceGroup { get; set; }

        [JsonProperty("IsPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public string UpdatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("NotificationSentDate")]
        public string NotificationSentDate { get; set; }

        [JsonProperty("Provinces")]
        public ProvinceSchema Provinces { get; set; }

        [JsonProperty("ArticleContentId")]
        public int? ArticleContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("TitleInEnglishDefault")]
        public string TitleInEnglishDefault { get; set; }
        

        [JsonProperty("TeaserText")]
        public  string  TeaserText { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

        [JsonProperty("RelatedResources")]
        public List<RelatedArticlesSchema> RelatedResources { get; set; }

        [JsonProperty("RelatedContacts")]
        public List<RelatedEntityId> RelatedContacts { get; set; }

        [JsonProperty("RelatedCountries")]
        public List<RelatedEntityId> RelatedCountries { get; set; }

        [JsonProperty("RelatedCountryGroups")]
        public List<RelatedEntityId> RelatedCountryGroups { get; set; }

        [JsonProperty("RelatedTaxTags")]
        public List<RelatedTaxTagsSchema> RelatedTaxTags { get; set; }

        [JsonProperty("RelatedArticles")]
        public List<RelatedArticlesSchema> RelatedArticles { get; set; }

        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; set; }
    }
}
