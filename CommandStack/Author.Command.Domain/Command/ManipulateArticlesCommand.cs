using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class ManipulateArticlesCommand :IRequest<ManipulateArticlesCommandResponse>
    {
        [JsonRequired]
        [JsonProperty("articlesIds")]
        public List<int> ArticlesIds { get; set; }

        [JsonRequired]
        [JsonProperty("operation")]
        public string Operation { get; set; }
    }
    public class ManipulateArticlesCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
