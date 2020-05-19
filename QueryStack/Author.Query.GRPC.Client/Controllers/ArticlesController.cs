using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Author.Query.GRPC.Client.Services;
using Microsoft.Extensions.Logging;
using System.Net;
using Author.Query.Persistence.DTO;
using Author.Query.Domain.Models;

namespace Author.Query.GRPC.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        public IArticleService _article;
        public ILogger<ArticlesController> _log;
        public ArticlesController(IArticleService article, ILogger<ArticlesController> log)
        {
            _article = article;
            _log = log;
        }


        [Route("getArticle")]
        [HttpPost]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ArticleDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ArticleDTO>> GetArticlebyId(GetArticleById objarticle)
        {
            if (objarticle.articleId == null)
            {
                return BadRequest("Need to pass ArticleId");
            }
            var article = await _article.getArticle(objarticle.articleId, objarticle.countryId, objarticle.userCookieId) ?? new ArticleDTO() { ArticleId = objarticle.articleId };
            return article;
        }
    }
}