using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class ManipulateCountryGroupsCommand : IRequest<ManipulateCountryGroupsCommandResponse> 
    {
        [JsonProperty("countryGroupIds")]
        public List<int> CountryGroupIds { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }
    }
    public class ManipulateCountryGroupsCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
