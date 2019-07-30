using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class UpdateCountryGroupsCommand : IRequest<UpdateCountryGroupsCommandResponse>
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [JsonProperty("countryGroupsId")]
        public int CountryGroupsId { get; set; }

        [JsonProperty("languageNames")]
        public List<LanguageName> LanguageName { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("imagesData")]
        public List<int> CountryIds { get; set; }
    }
    public class UpdateCountryGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
