using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Command.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorAdmin.Command.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentDisclaimerController : BaseController
    {
        private readonly IMediator _mediator;

        public ContentDisclaimerController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Create ContentDisclaimer
        /// </summary>
        /// <remarks>This API will create ContentDisclaimer</remarks>
        /// <param name="command">Create ContentDisclaimer command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcontentdisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateContentDisclaimer([FromBody] CreateContentDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }

        /// <summary>
        /// Update ContentDisclaimer
        /// </summary>
        /// <remarks>This API will update ContentDisclaimer</remarks>
        /// <param name="command">Update ContentDisclaimer command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatecontentdisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateContentDisclaimer([FromBody] UpdateContentDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }

        /// <summary>
        /// Delete ContentDisclaimer
        /// </summary>
        /// <remarks>This API will delete ContentDisclaimer</remarks>
        /// <param name="command">Delete ContentDisclaimer command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecontentdisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteContentDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}