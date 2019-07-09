using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Command.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthorAdmin.Command.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceGroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger<ResourceGroupController> _log;
        public ResourceGroupController(IMediator mediator, ILogger<ResourceGroupController> log)
        {
            _log = log;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("CreateResourceGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateResourceGroup([FromBody] CreateResourceGroupCommand command)
        {
            var response = await _mediator.Send(command);

            if (response == null)
            {
                _log.LogError("Error : " + response.FailureReason);
                return BadRequest();
            }

            if (response.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                _log.LogError("Error : " + response.FailureReason);
                return BadRequest(response.FailureReason);
            }
        }

        [HttpPost]
        [Route("UpdateResourceGroup")]
        [ProducesResponseType(typeof(string),201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateResourceGroup([FromBody] UpdateResourceGroupCommand command)
        {
            var response = await _mediator.Send(command);

            if (response == null)
            {
                _log.LogError("Error : " + response.FailureReason);
                return BadRequest();
            }

            if (response.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                _log.LogError("Error : " + response.FailureReason);
                return BadRequest(response.FailureReason);
            }
        }
    }
}