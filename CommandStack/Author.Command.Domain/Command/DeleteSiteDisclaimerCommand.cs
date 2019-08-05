using Author.Command.Domain.Models;
using Author.Core.Framework.Utilities.Validators;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class DeleteSiteDisclaimerCommand : IRequest<DeleteSiteDisclaimerCommandResponse>
    {
        [Required]
        [EnsureAllListElementsArePositiveIntegers]
        public List<int> SiteDisclaimerIds { get; set; }
    }
    public class DeleteSiteDisclaimerCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
