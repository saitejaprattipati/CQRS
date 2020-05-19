using Author.Query.GRPC.Client.Config;
using Author.Query.Persistence.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AuthorGRPC;

namespace Author.Query.GRPC.Client.Services
{
    public class ArticleService: IArticleService
    {
        private readonly UrlsConfig _urls;
        public readonly HttpClient _httpClient;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(HttpClient httpClient, IOptions<UrlsConfig> config, ILogger<ArticleService> logger)
        {
            _urls = config.Value;
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<ArticleDTO> getArticle(int articleId, int countryId, string userCookieId)
        {
            return await GrpcCallerService.CallService(_urls.GrpcAuthor, async channel =>
            {
                var client = new AuthorGRPC.Author.AuthorClient(channel);
              //  _logger.LogDebug("grpc client created, request = {@id}", articleId);
                var response = await client.GetArticlesAsync(new ArticleRequest { ArticleId = articleId, CountryId = countryId, UserCookieId= userCookieId});
                _logger.LogDebug("grpc response {@response}", response);
                return new ArticleDTO{ArticleId=response.ArticleId, PublishedDate=response.PublishedDate, Author= response.Author, ResourcePosition= response.ResourcePosition, Province= response.Province, LanguageId= response.LanguageId, Title= response.Title, TeaserText=response.TeaserText, ContentType=response.ContentType, Content=response.Content, ImageCredit=response.ImageCredit, ImageDescriptionText=response.ImageDescriptionText, IsRead=response.IsRead, Saved=response.Saved, SavedDate=response.SavedDate, SharingWebURL=response.SharingWebURL, ImagePath=response.ImagePath, Subscribed=response.Subscribed, Name=response.Name, Path=response.Path, CompleteResponse=response.CompleteResponse, TitleInEnglishDefault=response.TitleInEnglishDefault, ContainsYoutubeLink=response.ContainsYoutubeLink }; 
            });
        }
    }
}
