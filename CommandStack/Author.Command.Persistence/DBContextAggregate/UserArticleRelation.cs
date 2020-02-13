using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Persistence.DBContextAggregate
{
   public class UserArticleRelation
    {
        public Articles Article;
        public WebsiteUsers WebsiteUser;
        public TaxTags TaxTag;
        public Countries Country;
    }
}
