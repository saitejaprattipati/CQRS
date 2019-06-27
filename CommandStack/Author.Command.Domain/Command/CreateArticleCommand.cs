using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;

namespace Author.Command.Domain.Command
{
    public class CreateArticleCommand : IRequest<CreateArticleCommandResponse>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("articleName")]
        public string ArticleName { get; set; }
        [JsonProperty("articleCountry")]
        public string ArticleCountry { get; set; }

        [JsonProperty("articleID")]
        public string ArticleID { get; set; }
    }
    public class CreateArticleCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
