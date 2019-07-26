using Author.Command.Domain.Models;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class UpdateContentDisclaimerCommand: IRequest<UpdateContentDisclaimerCommandResponse>
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int DisclaimerId { get; set; }

        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string GroupName { get; set; }

        public List<DisclaimerContent> DisclaimerContent { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int DefaultCountryId { get; set; }
    }
    public class UpdateContentDisclaimerCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
