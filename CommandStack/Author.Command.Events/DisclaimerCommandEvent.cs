using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class DisclaimerCommandEvent:IntegrationEvent
    {
        [JsonProperty("DisclaimerId")]
        public int DisclaimerId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("DefaultCountryId")]
        public int? DefaultCountryId { get; set; }

        [JsonProperty("DisclaimerContentId")]
        public int DisclaimerContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("ProviderName")]
        public string ProviderName { get; set; }

        [JsonProperty("ProviderTerms")]
        public string ProviderTerms { get; set; }

        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; set; }

    }
}
