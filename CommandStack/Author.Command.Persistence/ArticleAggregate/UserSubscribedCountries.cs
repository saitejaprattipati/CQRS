using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class UserSubscribedCountries
    {
        public UserSubscribedCountries()
        {
            UserSubscribedCountryTags = new HashSet<UserSubscribedCountryTags>();
        }

        public int UserSubscribedCountryId { get; set; }
        public int? WebsiteUserId { get; set; }
        public int? CountryId { get; set; }

        public Countries Country { get; set; }
        public WebsiteUsers WebsiteUser { get; set; }
        public ICollection<UserSubscribedCountryTags> UserSubscribedCountryTags { get; set; }
    }
}
