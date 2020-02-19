using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class ResourceGroupCommandEvent:IntegrationEvent
    {
        [JsonProperty("ResourceGroupId")]
        public int ResourceGroupId { get; set; }

        [JsonProperty("IsPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty("Position")]
        public int Position { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [JsonProperty("ResourceGroupContentId")]
        public int ResourceGroupContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("GroupName")]
        public string GroupName { get; set; }

        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; set; }
    }
}
