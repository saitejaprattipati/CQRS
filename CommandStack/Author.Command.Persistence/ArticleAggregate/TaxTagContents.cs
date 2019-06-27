using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class TaxTagContents
    {
        public int TaxTagContentId { get; set; }
        public string DisplayName { get; set; }
        public int? LanguageId { get; set; }
        public int? TaxTagId { get; set; }

        public Languages Language { get; set; }
        public TaxTags TaxTag { get; set; }
    }
}
