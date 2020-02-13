using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Author.Command.Domain.Models;

namespace Author.Command.Domain.Command
{
    public class CreateArticleCommand : IRequest<CreateArticleCommandResponse>
    {
        [JsonProperty("articleID")]
        public string ArticleID { get; set; }

        [JsonProperty("articleName")]
        public string ArticleName { get; set; }

        [JsonProperty("imageId")]
        public int ImageId { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("provinceId")]
        public int ProvinceId { get; set; }

        [JsonProperty("isPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("subType")]
        public int? SubType { get; set; }

        [JsonProperty("sendNotification")]
        public bool SendNotification { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("notificationSentDate")]
        public DateTime? NotificationSentDate { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("disclaimerId")]
        public int DisclaimerId { get; set; }

        [JsonProperty("publishedDate")]
        public DateTime? PublishedDate { get; set; }

        [JsonProperty("resourceGroupId")]
        public int ResourceGroupId { get; set; }

        [JsonProperty("resourcePosition")]
        public int ResourcePosition { get; set; }

        [JsonProperty("articleContent")]
        public List<ArticleContent> ArticleContent { get; set; }

        [JsonProperty("relatedTaxTags")]
        public List<int> RelatedTaxTags { get; set; }

        [JsonProperty("relatedCountries")]
        public List<int> RelatedCountries { get; set; }

        [JsonProperty("relatedCountryGroups")]
        public List<int> RelatedCountryGroups { get; set; }

        [JsonProperty("relatedArticles")]
        public List<int> RelatedArticles { get; set; }

        [JsonProperty("relatedResources")]
        public List<int> RelatedResources { get; set; }

        [JsonProperty("relatedContacts")]
        public List<int> RelatedContacts { get; set; }
    }
    public class CreateArticleCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }   
}
