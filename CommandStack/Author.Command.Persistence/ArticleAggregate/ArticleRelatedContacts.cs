using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ArticleRelatedContacts
    {
        public int ArticleId { get; set; }
        public int ContactId { get; set; }

        public Articles Article { get; set; }
        public Contacts Contact { get; set; }
    }
}
