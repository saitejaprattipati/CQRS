using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ArticleRelatedCountryGroups
    {
        public int ArticleId { get; set; }
        public int CountryGroupId { get; set; }

        public Articles Article { get; set; }
        public CountryGroups CountryGroup { get; set; }
    }
}
