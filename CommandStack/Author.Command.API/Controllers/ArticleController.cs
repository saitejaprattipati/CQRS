using Author.Command.Domain.Command;
//using CommandDomain.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;

namespace Author.Command.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Articles")]
    public class ArticleController : Controller
    {
        //  private readonly ILogger _logger;
        private readonly IMediator _mediator;
        public ArticleController(IMediator mediator)
        {
            //    _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("CreateArticle")]
        [ProducesResponseType(typeof(string),201)] 
        [ProducesResponseType(typeof(string),400)]  
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);

                if (response != null)
                {
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
            catch (Exception ex)
            {
                //  _logger.LogError();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("UpdateArticle")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticleCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);

                if (response != null)
                {
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
            catch (Exception ex)
            {
                //  _logger.LogError();
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("ManipulateArticle")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ManipulateArticle([FromBody] ManipulateArticlesCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);

                if (response != null)
                {
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
            catch (Exception ex)
            {
                //  _logger.LogError();
            }
            return BadRequest();
        }
    }
}