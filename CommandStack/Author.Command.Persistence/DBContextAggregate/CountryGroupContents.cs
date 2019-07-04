using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class CountryGroupContents
    {
        public int CountryGroupContentId { get; set; }
        public int? LanguageId { get; set; }
        public string GroupName { get; set; }
        public int? CountryGroupId { get; set; }

        public virtual CountryGroups CountryGroup { get; set; }
        public virtual Languages Language { get; set; }
    }
}
