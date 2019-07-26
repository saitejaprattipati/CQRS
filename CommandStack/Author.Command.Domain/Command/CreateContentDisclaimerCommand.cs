using Author.Command.Domain.Models;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class CreateContentDisclaimerCommand:IRequest<CreateContentDisclaimerCommandResponse>
    {
        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string GroupName { get; set; }

        public List<DisclaimerContent> DisclaimerContent { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int DefaultCountryId { get; set; }
    }

    public class CreateContentDisclaimerCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
