using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Query.Persistence;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AuthorGRPC;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Author.Query.GRPC.Server
{
    public class ArticlesService : AuthorGRPC.Author.AuthorBase
    {
        private readonly ILogger<ArticlesService> _logger;
        private readonly IArticleService _repo;
        public ArticlesService(ILogger<ArticlesService> logger, IArticleService repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public override async Task<ArticleResponse> GetArticles(ArticleRequest request, ServerCallContext context)
        {
            var data = await _repo.GetArticleAsync(request.ArticleId, request.CountryId, request.UserCookieId);

            if (data != null)
            {
                context.Status = new Status(StatusCode.OK, $"Article with id {request.ArticleId} do exist");
                return new ArticleResponse { ArticleId = data.ArticleId, PublishedDate = data.PublishedDate, Author = data.Author, ResourcePosition = data.ResourcePosition, Province = data.Province, LanguageId = data.LanguageId, Title = data.Title, TeaserText = data.TeaserText, ContentType = data.ContentType, Content = data.Content, ImageCredit = data.ImageCredit, ImageDescriptionText = data.ImageDescriptionText, IsRead = data.IsRead, Saved = data.Saved, SavedDate = data.SavedDate, SharingWebURL = "", ImagePath = data.ImagePath, Subscribed = data.Subscribed, Name = "", Path = "", CompleteResponse = data.CompleteResponse, TitleInEnglishDefault = data.TitleInEnglishDefault, ContainsYoutubeLink = data.ContainsYoutubeLink };
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $"Article with id {request.ArticleId} do not exist");
            }

            return new ArticleResponse { };
        }
    }
}
