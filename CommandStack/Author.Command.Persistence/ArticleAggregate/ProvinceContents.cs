using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ProvinceContents
    {
        public int ProvinceContentId { get; set; }
        public string DisplayName { get; set; }
        public int ProvinceId { get; set; }
        public int LanguageId { get; set; }

        public Languages Language { get; set; }
        public Provinces Province { get; set; }
    }
}
