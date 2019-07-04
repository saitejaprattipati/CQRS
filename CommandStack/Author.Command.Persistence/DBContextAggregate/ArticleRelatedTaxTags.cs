using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ArticleRelatedTaxTags
    {
        public int ArticleId { get; set; }
        public int TaxTagId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual TaxTags TaxTag { get; set; }
    }
}
