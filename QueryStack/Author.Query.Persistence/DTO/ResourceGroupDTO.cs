using Author.Core.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class ResourceGroupResult
    {
        public ResourceGroupResult()
        {
            ResourceGroups = new List<ResourceGroupDTO>();
        }
        public List<ResourceGroupDTO> ResourceGroups { get; set; }
    }
    public class ResourceGroupDTO
    {
        public int ResourceGroupId { get; set; }

        public int Position { get; set; }

        [JsonProperty("groupname")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string GroupName { get; set; }
        public int LanguageId { get; set; }
    }
}
