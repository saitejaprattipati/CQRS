using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//using CommandDomain.Command;
using MediatR;
using Author.Command.Domain.Command;
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
    }
}