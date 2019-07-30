using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class UpdateTagsCommand : IRequest<UpdateTagGroupsCommandResponse>
    {
        [JsonProperty("tagGroupsId")]
        public int TagGroupsId { get; set; }

        [JsonProperty("languageName")]
        public List<LanguageName> LanguageName { get; set; }
        [JsonProperty("tagType")]
        public string TagType { get; set; }
        [JsonProperty("tagGroup")]
        public int TagGroup { get; set; }
        [JsonProperty("relatedCountries")]
        public List<int> RelatedCountyIds { get; set; }
    }
    public class UpdateTagGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
