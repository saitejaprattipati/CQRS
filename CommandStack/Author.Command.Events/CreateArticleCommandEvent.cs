using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;


namespace Author.Command.Events
{
   public class CreateArticleCommandEvent : IntegrationEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("articleName")]
        public string ArticleName { get; set; }
        [JsonProperty("articleCountry")]
        public string ArticleCountry { get; set; }

        [JsonProperty("articleID")]
        public Guid ArticleID { get; set; }
        public CreateArticleCommandEvent()
        {
            ArticleID = Guid.NewGuid();
        }
    }
}
