using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Author.Command.Domain.Models
{
    public class ResourceGroupsSchema
    {
        [JsonProperty("ResourceGroupId")]
        public int ResourceGroupId { get; set; }
        [JsonProperty("Position")]
        public int Position { get; set; }
        [JsonProperty("GroupName")]
        public string GroupName { get; set; }
    }
}
