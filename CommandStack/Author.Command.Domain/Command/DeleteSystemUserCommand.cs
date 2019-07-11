using Author.Command.Domain.Models;
using Author.Core.Framework.Utilities.Validators;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Command
{
    public class DeleteSystemUserCommand : IRequest<DeleteSystemUserCommandResponse>
    {
        [Required]
        [EnsureAllListElementsArePositiveIntegers]
        public List<int> SystemUserIds { get; set; }
    }
    public class DeleteSystemUserCommandResponse : CommandResponse
    {
        [JsonProperty("UpdatedStatus")]
        public string UpdatedStatus { get; set; }
    }
}
