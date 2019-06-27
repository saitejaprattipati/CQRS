using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ResourceGroupContents
    {
        public int ResourceGroupContentId { get; set; }
        public int? LanguageId { get; set; }
        public string GroupName { get; set; }
        public int? ResourceGroupId { get; set; }

        public Languages Language { get; set; }
        public ResourceGroups ResourceGroup { get; set; }
    }
}
