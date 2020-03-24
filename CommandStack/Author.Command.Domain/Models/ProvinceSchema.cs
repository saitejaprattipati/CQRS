using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Author.Command.Domain.Models
{
    public class ProvinceSchema
    {
        [JsonProperty("ProvinceId")]
        public int ProvinceId { get; set; }
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
    }
}
