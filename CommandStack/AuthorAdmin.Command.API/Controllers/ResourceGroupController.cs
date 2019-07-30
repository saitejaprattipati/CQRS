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
    public class ResourceGroupController : BaseController
    {
        private readonly IMediator _mediator;
        private ILogger<ResourceGroupController> _log;
        public ResourceGroupController(IMediator mediator, ILogger<ResourceGroupController> log)
        {
            _log = log;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        /// <summary>
        /// Create Resource Group
        /// </summary>
        /// <remarks>This API will create resource group</remarks>
        /// <param name="command">Create resource group command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createResourceGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateResourceGroup([FromBody] CreateResourceGroupCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Update Resource Group
        /// </summary>
        /// <remarks>This API will update resource group</remarks>
        /// <param name="command">Update resource group command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateResourceGroup")]
        [ProducesResponseType(typeof(string),201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateResourceGroup([FromBody] UpdateResourceGroupCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Manipulate Resource Group
        /// </summary>
        /// <remarks>This API will manipulate resource group</remarks>
        /// <param name="command">Manipulate resource group command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("manipulateResourceGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ManipulateResourceGroup([FromBody] ManipulateResourceGroupCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}