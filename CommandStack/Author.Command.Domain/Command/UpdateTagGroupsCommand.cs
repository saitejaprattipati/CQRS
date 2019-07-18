using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class UpdateTagGroupsCommand : IRequest<UpdateTagGroupsCommandResponse>
    {
        [JsonProperty("tagGroupsId")]
        public int TagGroupsId { get; set; }

        [JsonProperty("languageName")]
        public List<LanguageName> LanguageName { get; set; }
    }
    public class UpdateTagGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
