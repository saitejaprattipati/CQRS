using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class ManipulateCountriesCommand : IRequest<ManipulateCountriesCommandResponse>
    {
        [JsonProperty("resourceGroupIds")]
        public List<int> ResourceGroupIds { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }
    }
    public class ManipulateCountriesCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
