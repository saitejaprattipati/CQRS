using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ArticleRelatedCountries
    {
        public int ArticleId { get; set; }
        public int CountryId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual Countries Country { get; set; }
    }
}
