using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class CreateTagsCommand : IRequest<TagsCommandResponse>
    {
        [JsonProperty("languageName")]
        public List<LanguageName> LanguageName { get; set; }

        [JsonProperty("tagType")]
        public string TagType { get; set; }
        [JsonProperty("tagGroup")]
        public int TagGroup { get; set; }
        [JsonProperty("relatedCountries")]
        public List<int> RelatedCountyIds { get; set; }
    }
    public class TagsCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
