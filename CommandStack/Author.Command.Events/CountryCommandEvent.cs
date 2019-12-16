using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class CountryCommandEvent:IntegrationEvent
    {
        [JsonProperty("CountryId")]
        public int CountryId { get; set; }

        [JsonProperty("SVGImageId")]
        public int? SVGImageId { get; set; }

        [JsonProperty("PNGImageId")]
        public int? PNGImageId { get; set; }

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

        [JsonProperty("CountryContentId")]
        public int CountryContentId { get; set; }

        [JsonProperty("LanguageId")]
        public int? LanguageId { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("DsiplayNameShort")]
        public string DsiplayNameShort { get; set; }
    }
}
