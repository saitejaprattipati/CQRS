using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class UserSavedArticles
    {
        public int UserSavedArticleId { get; set; }
        public int? WebsiteUserId { get; set; }
        public int? ArticleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public Articles Article { get; set; }
        public WebsiteUsers WebsiteUser { get; set; }
    }
}
