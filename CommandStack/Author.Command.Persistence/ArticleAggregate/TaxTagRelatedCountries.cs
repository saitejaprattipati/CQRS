using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class TaxTagRelatedCountries
    {
        public int TaxTagId { get; set; }
        public int CountryId { get; set; }

        public Countries Country { get; set; }
        public TaxTags TaxTag { get; set; }
    }
}
