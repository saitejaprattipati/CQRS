using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class AzureSearchIndex
    {
        public long AzureSearchIndexId { get; set; }
        public DateTime AzureSearchIndexUpdatedDate { get; set; }
        public int? ArticleId { get; set; }
        public int? ArticleContentId { get; set; }
        public string ArticleAuthor { get; set; }
        public string ImagePath { get; set; }
        public int? LanguageId { get; set; }
        public string Title { get; set; }
        public string Teaser { get; set; }
        public string Content { get; set; }
        public int? ContactId { get; set; }
        public string ContactName { get; set; }
        public int? TagId { get; set; }
        public string TagName { get; set; }
        public bool? Status { get; set; }
    }
}
