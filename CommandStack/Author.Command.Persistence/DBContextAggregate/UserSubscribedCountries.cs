using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
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

        public virtual Countries Country { get; set; }
        public virtual WebsiteUsers WebsiteUser { get; set; }
        public virtual ICollection<UserSubscribedCountryTags> UserSubscribedCountryTags { get; set; }
    }
}
