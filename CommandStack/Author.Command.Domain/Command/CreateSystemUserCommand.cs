using Author.Command.Domain.Models;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class CreateSystemUserCommand : IRequest<CreateSystemUserCommandResponse>
    {
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

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        [Required]
        public string Email { get; set; }

        [Required]
        public SystemUserRole Role { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int HomeCountry { get; set; }

        [Required]
        public List<int> Countries { get; set; }
    }

    public class CreateSystemUserCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
