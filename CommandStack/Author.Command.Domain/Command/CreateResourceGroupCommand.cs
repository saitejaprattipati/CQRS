using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class CreateResourceGroupCommand : IRequest<CreateResourceGroupCommandResponse>
    {

        [JsonProperty("LanguageName")]
        public List<LanguageName> languageName { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("position")]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
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
