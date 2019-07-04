using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class TaxTagRelatedCountries
    {
        public int TaxTagId { get; set; }
        public int CountryId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual TaxTags TaxTag { get; set; }
    }
}
