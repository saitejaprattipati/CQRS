using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class UserReadArticles
    {
        public int UserReadArticleId { get; set; }
        public int? WebsiteUserId { get; set; }
        public int? ArticleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Articles Article { get; set; }
        public virtual WebsiteUsers WebsiteUser { get; set; }
    }
}
