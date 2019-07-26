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
    public class UserController : BaseController//ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        /// <summary>
        /// Create SystemUser
        /// </summary>
        /// <remarks>This API will create SystemUser</remarks>
        /// <param name="command">Create SystemUser command object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createuser")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateSystemUserCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }

        /// <summary>
        /// Update SystemUser
        /// </summary>
        /// <remarks>This API will update SystemUser</remarks>
        /// <param name="command">Update SystemUser command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateuser")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateSystemUserCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <remarks>This API will delete SystemUser</remarks>
        /// <param name="command">Delete SystemUser command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteuser")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteSystemUserCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
    }
}