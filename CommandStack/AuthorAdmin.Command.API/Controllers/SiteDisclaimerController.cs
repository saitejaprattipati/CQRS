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
    public class SiteDisclaimerController : BaseController
    {
        private readonly IMediator _mediator;

        public SiteDisclaimerController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Create SiteDisclaimer
        /// </summary>
        /// <remarks>This API will create SiteDisclaimer</remarks>
        /// <param name="command">Create SiteDisclaimer command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createsitedisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSiteDisclaimer([FromBody] CreateSiteDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
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
            return CreateResponse(response);
        }

        /// <summary>
        /// Delete SiteDisclaimer
        /// </summary>
        /// <remarks>This API will delete SiteDisclaimer</remarks>
        /// <param name="command">Delete SiteDisclaimer command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletesitedisclaimer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteSiteDisclaimerCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}