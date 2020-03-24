using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Author.Command.Domain.Models
{
    public class DisclamersSchema
    {
        [JsonProperty("DisclaimerId")]
        public int DisclaimerId { get; set; }
        [JsonProperty("ProviderName")]
        public string ProviderName { get; set; }
        [JsonProperty("ProviderTerms")]
        public string ProviderTerms { get; set; }
    }
}
