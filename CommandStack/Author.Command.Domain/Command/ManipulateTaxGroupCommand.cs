using Author.Command.Domain.Models;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
   public class ManipulateTaxGroupCommand : IRequest<ManipulateTaxGroupCommandResponse>
    {
        [JsonProperty("taxGroupIds")]
        public List<int> TaxGroupIds { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }

        [JsonProperty("tagType")]
        public string TagType { get; set; }
    }
    public class ManipulateTaxGroupCommandResponse : CommandResponse
    {
        [JsonProperty("updatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
