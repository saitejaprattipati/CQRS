using Author.Command.Domain.Models;
using Author.Core.Framework;
using Author.Core.Framework.Utilities.Validators;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class UpdateSystemUserCommand: IRequest<UpdateSystemUserCommandResponse>
    {
       [Required]
       [Range(0,int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int SystemUserId { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        [Required]
        public string FirstName { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        [Required]
        public string LastName { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Level { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string WorkPhoneNumber { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string MobilePhoneNumber { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        [Required]
        public string Location { get; set; }

        [Required]
        //[EmailAddress(ErrorMessage ="Invalid EmailId")]
        [EmailValidator]
        public string Email { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "Role not valid")]
        public int Role { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int HomeCountry { get; set; }

        [Required]
        [EnsureAllListElementsArePositiveIntegers]
        public List<int> Countries { get; set; }
    }

    public class UpdateSystemUserCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
