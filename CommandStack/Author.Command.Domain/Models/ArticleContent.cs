using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Domain.Models
{
    public class ArticleContent
    {
        public string Content { get; set; }
        public int LanguageId { get; set; }
        public string TeaserText { get; set; }
        public string Title { get; set; }
    }
}
