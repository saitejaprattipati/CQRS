using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class TagGroupCommandEvent:IntegrationEvent
    {
        [JsonProperty("TaxTagId")]
        public int TagId { get; set; }

        [JsonProperty("ParentTagId")]
        public int? ParentTagId { get; set; }

        [JsonProperty("IsPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("RelatedCountryIds")]
        public List<int> RelatedCountryIds { get; set; }

        [JsonProperty("TaxTagContentId")]
        public int TagContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; set; }
    }
}
