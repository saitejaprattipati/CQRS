using Author.Command.Domain.Models;
using Author.Core.Framework.Utilities.Validators;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class DeleteContentDisclaimerCommand:IRequest<DeleteContentDisclaimerCommandResponse>
    {
        [Required]
        [EnsureAllListElementsArePositiveIntegers]
        public List<int> DisclaimerIds { get; set; }
    }
    public class DeleteContentDisclaimerCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
