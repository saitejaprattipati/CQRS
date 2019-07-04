using Author.Command.Domain.Models;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class SystemUserViewCommand : IRequest<CreateSystemUserCommandResponse>
    {
        public int Id { get; set; }

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

        public TimeSpan? TimeZone { get; set; }

        [Required]
        public int HomeCountry { get; set; }

        [Required]
        public List<int> Countries { get; set; }

        //[RegularExpression(Constants.GeneralStringRegularExpression)]
        //public string CreatedBy { get; set; }

        //[RegularExpression(Constants.GeneralStringRegularExpression)]
        //public string UpdatedBy { get; set; }
    }

    public class CreateSystemUserCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
