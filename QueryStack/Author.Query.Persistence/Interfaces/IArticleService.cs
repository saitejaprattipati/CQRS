using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleDTO> GetArticleAsync(int articleId, int countryId);

        Task<ILookup<int, Articles>> GetRelatedArticles(IEnumerable<int> articleIds);

    }
}
