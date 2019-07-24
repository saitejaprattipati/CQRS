using Author.Command.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthorAdmin.Command.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteDisclaimerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SiteDisclaimerController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Update SiteDisclaimer
        /// </summary>
        /// <remarks>This API will update SiteDisclaimer</remarks>
        /// <param name="command">Update SiteDisclaimer command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatesitedisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSiteDisclaimer([FromBody] UpdateSiteDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);

            if (response == null)
            {
                return BadRequest();
            }

            if (response.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.FailureReason);
            }
        }
    }
}