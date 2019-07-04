using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class TaxTagContents
    {
        public int TaxTagContentId { get; set; }
        public string DisplayName { get; set; }
        public int? LanguageId { get; set; }
        public int? TaxTagId { get; set; }

        public virtual Languages Language { get; set; }
        public virtual TaxTags TaxTag { get; set; }
    }
}
