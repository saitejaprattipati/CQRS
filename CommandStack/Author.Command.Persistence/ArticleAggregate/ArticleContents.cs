using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ArticleContents
    {
        public int ArticleContentId { get; set; }
        public int? LanguageId { get; set; }
        public string Title { get; set; }
        public string TeaserText { get; set; }
        public string Content { get; set; }
        public int? ArticleId { get; set; }

        public Articles Article { get; set; }
        public Languages Language { get; set; }
    }
}
