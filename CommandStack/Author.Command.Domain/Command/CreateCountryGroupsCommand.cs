using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class CreateCountryGroupsCommand : IRequest<CreateCountryGroupsCommandResponse>
    {
        [JsonProperty("languageNames")]
        public List<LanguageName> LanguageName { get; set; }
        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("imagesData")]
        public List<int> CountryIds { get; set; }
    }
    public class CreateCountryGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
