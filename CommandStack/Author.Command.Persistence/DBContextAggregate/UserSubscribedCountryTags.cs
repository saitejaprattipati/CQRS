using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class UserSubscribedCountryTags
    {
        public int UserSubscribedCountryId { get; set; }
        public int TaxTagId { get; set; }

        public virtual TaxTags TaxTag { get; set; }
        public virtual UserSubscribedCountries UserSubscribedCountry { get; set; }
    }
}
