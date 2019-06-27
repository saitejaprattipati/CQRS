using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class CountryContents
    {
        public int CountryContentId { get; set; }
        public int? LanguageId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameShort { get; set; }
        public int? CountryId { get; set; }

        public Countries Country { get; set; }
        public Languages Language { get; set; }
    }
}
