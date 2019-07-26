using Author.Command.Domain.Models;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class UpdateSiteDisclaimerCommand:IRequest<UpdateSiteDisclaimerCommandResponse>
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int SiteDisclaimerId { get; set; }

        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string PublishedDate { get; set; }

        public string Author { get; set; }

        public List<LanguageContent> LanguageContent { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int ArticleType { get; set; }

        public bool SendNotification { get; set; } = false;
    }

    public class UpdateSiteDisclaimerCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
