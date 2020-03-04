using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Author.Command.Domain.Models
{
    public class RelatedArticlesSchema
    {
        [JsonProperty("ArticleId")]
        public int ArticleId { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("CountryId")]
        public List<RelatedEntityId> CountryId { get; set; }
        [JsonProperty("PublishedDate")]
        public string PublishedDate { get; set; }

    }
}
