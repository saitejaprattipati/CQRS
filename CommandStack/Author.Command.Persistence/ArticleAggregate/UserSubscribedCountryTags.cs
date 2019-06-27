using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class UserSubscribedCountryTags
    {
        public int UserSubscribedCountryId { get; set; }
        public int TaxTagId { get; set; }

        public TaxTags TaxTag { get; set; }
        public UserSubscribedCountries UserSubscribedCountry { get; set; }
    }
}
