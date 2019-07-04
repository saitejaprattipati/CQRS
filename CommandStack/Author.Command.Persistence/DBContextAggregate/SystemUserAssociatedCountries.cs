using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class SystemUserAssociatedCountries
    {
        public int SystemUserId { get; set; }
        public int CountryId { get; set; }
        public bool IsPrimary { get; set; }
        public int SystemUserAssociatedCountryId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual SystemUsers SystemUser { get; set; }
    }
}
