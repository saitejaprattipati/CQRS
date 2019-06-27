using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class RelatedResources
    {
        public int ArticleId { get; set; }
        public int RelatedArticleId { get; set; }

        public Articles Article { get; set; }
        public Articles RelatedArticle { get; set; }
    }
}
