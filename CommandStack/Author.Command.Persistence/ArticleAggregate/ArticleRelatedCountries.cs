using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ArticleRelatedCountries
    {
        public int ArticleId { get; set; }
        public int CountryId { get; set; }

        public Articles Article { get; set; }
        public Countries Country { get; set; }
    }
}
