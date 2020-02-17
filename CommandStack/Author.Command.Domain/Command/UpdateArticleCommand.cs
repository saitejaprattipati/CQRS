using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Author.Command.Domain.Models;

namespace Author.Command.Domain.Command
{
   public class UpdateArticleCommand : IRequest<UpdateArticleCommandResponse>
    {
        [JsonRequired]
        [JsonProperty("articleID")]
        public int ArticleID { get; set; }

        [JsonRequired]
        [JsonProperty("articleName")]
        public string ArticleName { get; set; }

        [JsonRequired]
        [JsonProperty("imageId")]
        public int ImageId { get; set; }

        [JsonRequired]
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonRequired]
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonRequired]
        [JsonProperty("provinceId")]
        public int ProvinceId { get; set; }

        [JsonRequired]
        [JsonProperty("isPublished")]
        public bool IsPublished { get; set; }

        [JsonRequired]
        [JsonProperty("subType")]
        public int SubType { get; set; }

        [JsonRequired]
        [JsonProperty("sendNotification")]
        public bool SendNotification { get; set; }

        [JsonRequired]
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("notificationSentDate")]
        public DateTime NotificationSentDate { get; set; }

        [JsonRequired]
        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonRequired]
        [JsonProperty("disclaimerId")]
        public int DisclaimerId { get; set; }

        [JsonProperty("publishedDate")]
        public DateTime? PublishedDate { get; set; }

        [JsonRequired]
        [JsonProperty("resourceGroupId")]
        public int ResourceGroupId { get; set; }

        [JsonRequired]
        [JsonProperty("resourcePosition")]
        public int ResourcePosition { get; set; }

        [JsonRequired]
        [JsonProperty("articleContent")]
        public List<ArticleContent> ArticleContent { get; set; }

        [JsonRequired]
        [JsonProperty("relatedTaxTags")]
        public List<int> RelatedTaxTags { get; set; }

        [JsonRequired]
        [JsonProperty("relatedCountries")]
        public List<int> RelatedCountries { get; set; }

        [JsonRequired]
        [JsonProperty("relatedCountryGroups")]
        public List<int> RelatedCountryGroups { get; set; }
        
        [JsonRequired]
        [JsonProperty("relatedArticles")]
        public List<int> RelatedArticles { get; set; }

        [JsonRequired]
        [JsonProperty("relatedResources")]
        public List<int> RelatedResources { get; set; }

        [JsonRequired]
        [JsonProperty("relatedContacts")]
        public List<int> RelatedContacts { get; set; }
    }
    public class UpdateArticleCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
