using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Author.Command.Domain.Command
{
   public class CreateResourceGroupCommand : IRequest<CreateResourceGroupCommandResponse>
    {

        [JsonProperty("LanguageName")]
        public List<LanguageName> languageName { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
    public class CreateResourceGroupCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
    public class LanguageName
    {
        public string Language { get; set; }
        public string Name { get; set; }
    }
}
