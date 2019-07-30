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
    public class TagsController : BaseController
    {
        private readonly IMediator _mediator;
        private ILogger<TagsController> _log;
        public TagsController(IMediator mediator, ILogger<TagsController> log)
        {
            _log = log;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        /// <summary>
        /// Create Tag Group
        /// </summary>
        /// <remarks>This API will create tag group</remarks>
        /// <param name="command">Create tag group command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateTagGroup([FromBody] CreateTagsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Update Tag Group
        /// </summary>
        /// <remarks>This API will update tag group</remarks>
        /// <param name="command">Update tag group command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateTagGroup([FromBody] UpdateTagsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Manipulate Tag Group
        /// </summary>
        /// <remarks>This API will manipulate Tag group</remarks>
        /// <param name="command">Manipulate Tag group command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("manipulateTagGroup")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> manipulateTagGroup([FromBody] ManipulateTaxGroupCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Create Tags
        /// </summary>
        /// <remarks>This API will create Tags</remarks>
        /// <param name="command">Create Tags command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateTags")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateTags([FromBody] CreateTagsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Update Tags
        /// </summary>
        /// <remarks>This API will update tags</remarks>
        /// <param name="command">Update tags command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateTags")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateTags([FromBody] UpdateTagsCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Manipulate Tags
        /// </summary>
        /// <remarks>This API will manipulate tags</remarks>
        /// <param name="command">Manipulate tags command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("manipulateTags")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ManipulateTags([FromBody] ManipulateTaxGroupCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}