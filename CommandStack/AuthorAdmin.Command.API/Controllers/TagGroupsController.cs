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
    public class TagGroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger<TagGroupsController> _log;
        public TagGroupsController(IMediator mediator, ILogger<TagGroupsController> log)
        {
            _log = log;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("CreateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateTagGroup([FromBody] CreateTagGroupsCommand command)
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
        [Route("updateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateTagGroup([FromBody] UpdateTagGroupsCommand command)
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
        [Route("manipulateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> manipulateTagGroup([FromBody] ManipulateTaxGroupCommand command)
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