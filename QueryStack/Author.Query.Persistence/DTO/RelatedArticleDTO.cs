using System.Collections.Generic;

namespace Author.Query.Persistence.DTO
{
    public class RelatedArticleDTO
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public List<CountryDTO> RelatedCountries { get; set; }
        public string PublishedDate { get; set; }
    }
}
