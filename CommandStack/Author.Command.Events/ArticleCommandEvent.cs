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
        public DateTime PublishedDate { get; set; }

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

        [JsonProperty("DisclaimerId")]
        public int? DisclaimerId { get; set; }

        [JsonProperty("ResourceGroupId")]
        public int? ResourceGroupId { get; set; }

        [JsonProperty("IsPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("NotificationSentDate")]
        public DateTime? NotificationSentDate { get; set; }

        [JsonProperty("ProvinceId")]
        public int? ProvinceId { get; set; }

        [JsonProperty("ArticleContentId")]
        public int? ArticleContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("TeaserText")]
        public  string  TeaserText { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("RelatedResources")]
        public List<RelatedEntityId> RelatedResources { get; set; }

        [JsonProperty("RelatedContacts")]
        public List<RelatedEntityId> RelatedContacts { get; set; }

        [JsonProperty("RelatedCountries")]
        public List<RelatedEntityId> RelatedCountries { get; set; }

        [JsonProperty("RelatedCountryGroups")]
        public List<RelatedEntityId> RelatedCountryGroups { get; set; }

        [JsonProperty("RelatedTaxTags")]
        public List<RelatedEntityId> RelatedTaxTags { get; set; }
        [JsonProperty("RelatedArticles")]
        public List<RelatedEntityId> RelatedArticles { get; set; }
    }
}
