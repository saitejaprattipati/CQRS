using Author.Query.Persistence.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.GRPC.Client.Services
{
   public interface IArticleService
    {
        Task<ArticleDTO> getArticle(int articleId, int countryId, string userCookieId);
    }
}
