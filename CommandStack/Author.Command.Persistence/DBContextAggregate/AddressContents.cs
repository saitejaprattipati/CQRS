using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class AddressContents
    {
        public int AddressContentId { get; set; }
        public int? LanguageId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? AddressId { get; set; }
        public bool StreetEdited { get; set; }
        public bool CityEdited { get; set; }
        public bool StateEdited { get; set; }
        public bool CountryEdited { get; set; }

        public virtual Addresses Address { get; set; }
        public virtual Languages Language { get; set; }
    }
}
