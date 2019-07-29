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
        /// <remarks>This API will create Country Groups</remarks>
        /// <param name="command">Create Country Groups command object</param>
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
        /// Create Country Groups
        /// </summary>
        /// <remarks>This API will create Country Groups</remarks>
        /// <param name="command">Create Country Groups command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateCountryGroups")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCountryGroups([FromBody] UpdateCountryGroupsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }        
    }
}