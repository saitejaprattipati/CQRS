using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class CreateCountryCommand : IRequest<CreateCountryCommandResponse>
    {
        [JsonProperty("languageNames")]
        public List<LanguageNames> LanguageNames { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("imagesData")]
        public ImagesData ImagesData { get; set; }
    }
    public class CreateCountryCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
    public class LanguageNames
    {
        public int? LanguageID { get; set; }
        public string CountryName { get; set; }
        public string ShortName { get; set; }
    }
    public class ImagesData
    {
        public string JPGData { get; set; }
        public string JPGName { get; set; }
        public string SVGData { get; set; }
        public string SVGName { get; set; }
    }
}
