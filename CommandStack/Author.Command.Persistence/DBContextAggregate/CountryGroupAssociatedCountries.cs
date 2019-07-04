using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class CountryGroupAssociatedCountries
    {
        public int CountryGroupId { get; set; }
        public int CountryId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual CountryGroups CountryGroup { get; set; }
    }
}
