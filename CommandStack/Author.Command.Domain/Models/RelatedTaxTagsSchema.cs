using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Author.Command.Domain.Models
{
    public class RelatedTaxTagsSchema
    {
        [JsonProperty("TaxTagId")]
        public int TaxTagId { get; set; }
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
    }
}
