using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class CountryContents
    {
        public int CountryContentId { get; set; }
        public int? LanguageId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameShort { get; set; }
        public int? CountryId { get; set; }

        public virtual Countries Country { get; set; }
        public virtual Languages Language { get; set; }
    }
}
