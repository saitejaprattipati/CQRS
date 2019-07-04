using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ArticleRelatedCountryGroups
    {
        public int ArticleId { get; set; }
        public int CountryGroupId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual CountryGroups CountryGroup { get; set; }
    }
}
