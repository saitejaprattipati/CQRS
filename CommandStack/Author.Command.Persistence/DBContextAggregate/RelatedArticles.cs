using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class RelatedArticles
    {
        public int ArticleId { get; set; }
        public int RelatedArticleId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual Articles RelatedArticle { get; set; }
    }
}
