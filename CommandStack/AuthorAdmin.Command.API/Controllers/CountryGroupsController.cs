using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Author.Command.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace AuthorAdmin.Command.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryGroupsController : BaseController
    {
        private readonly IMediator _mediator;

        public CountryGroupsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Create Country Groups
        /// </summary>
        /// <remarks>This API will create country groups</remarks>
        /// <param name="command">Create country groups command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createCountryGroups")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCountryGroups([FromBody] CreateCountryGroupsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Update Country Groups
        /// </summary>
        /// <remarks>This API will Update country groups</remarks>
        /// <param name="command">Update country groups command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateCountryGroups")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCountryGroups([FromBody] UpdateCountryGroupsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Manipilate Country Groups
        /// </summary>
        /// <remarks>This API will Manipilate country groups</remarks>
        /// <param name="command">Manipilate country groups command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("manipulateCountry")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ManipulateCountry([FromBody] ManipulateCountryGroupsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}