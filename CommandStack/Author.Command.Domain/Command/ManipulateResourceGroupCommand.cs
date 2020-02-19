using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class ManipulateResourceGroupCommand : IRequest<ManipulateResourceGroupCommandResponse>
    {
        [JsonProperty("resourceGroupIds")]
        public List<int> ResourceGroupIds { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }
    }
    public class ManipulateResourceGroupCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
