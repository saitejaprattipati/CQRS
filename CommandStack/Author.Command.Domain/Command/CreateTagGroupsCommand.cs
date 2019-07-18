using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class CreateTagGroupsCommand : IRequest<TagGroupsCommandResponse>
    {
        [JsonProperty("languageName")]
        public List<LanguageName> LanguageName { get; set; }
    }
    public class TagGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
