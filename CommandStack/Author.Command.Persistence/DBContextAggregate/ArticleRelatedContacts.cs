using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ArticleRelatedContacts
    {
        public int ArticleId { get; set; }
        public int ContactId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual Contacts Contact { get; set; }
    }
}
