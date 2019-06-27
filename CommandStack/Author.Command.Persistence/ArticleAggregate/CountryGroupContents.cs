using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class CountryGroupContents
    {
        public int CountryGroupContentId { get; set; }
        public int? LanguageId { get; set; }
        public string GroupName { get; set; }
        public int? CountryGroupId { get; set; }

        public CountryGroups CountryGroup { get; set; }
        public Languages Language { get; set; }
    }
}
