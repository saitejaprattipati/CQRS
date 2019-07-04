using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ResourceGroupContents
    {
        public int ResourceGroupContentId { get; set; }
        public int? LanguageId { get; set; }
        public string GroupName { get; set; }
        public int? ResourceGroupId { get; set; }

        public virtual Languages Language { get; set; }
        public virtual ResourceGroups ResourceGroup { get; set; }
    }
}
