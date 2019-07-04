using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ProvinceContents
    {
        public int ProvinceContentId { get; set; }
        public string DisplayName { get; set; }
        public int ProvinceId { get; set; }
        public int LanguageId { get; set; }

        public virtual Languages Language { get; set; }
        public virtual Provinces Province { get; set; }
    }
}
