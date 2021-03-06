using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class UpdateResourceGroupCommand : IRequest<UpdateResourceGroupCommandResponse>
    {
        [Required]
        [JsonProperty("resourceGroupId")]
        public int ResourceGroupId { get; set; }

        [JsonProperty("languageName")]
        public List<LanguageName> LanguageName { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("position")]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Position { get; set; }
    }
    public class UpdateResourceGroupCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
