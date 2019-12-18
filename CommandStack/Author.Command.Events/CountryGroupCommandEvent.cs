using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class CountryGroupCommandEvent:IntegrationEvent
    {
        [JsonProperty("CountryGroupId")]
        public int CountryGroupId { get; set; }

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

        [JsonProperty("CountryGroupContentId")]
        public int CountryGroupContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("GroupName")]
        public string GroupName { get; set; }

        [JsonProperty("AssociatedCountryIds")]
        public List<int> AssociatedCountryIds { get; set; }
    }
}
