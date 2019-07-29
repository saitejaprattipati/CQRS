using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class UpdateCountryCommand : IRequest<UpdateCountryCommandResponse>
    {
        [Required]
        [JsonProperty("CountryId")]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int CountryId { get; set; }

        [JsonProperty("languageNames")]
        public List<LanguageNames> LanguageNames { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("imagesData")]
        public ImagesData ImagesData { get; set; }
    }
    public class UpdateCountryCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
