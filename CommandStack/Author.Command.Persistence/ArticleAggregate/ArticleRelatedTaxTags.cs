using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ArticleRelatedTaxTags
    {
        public int ArticleId { get; set; }
        public int TaxTagId { get; set; }

        public Articles Article { get; set; }
        public TaxTags TaxTag { get; set; }
    }
}
