using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class AuthenticationUrlRequests
    {
        public Guid CookieId { get; set; }
        public DateTime CookieCreatedDate { get; set; }
        public string UserData { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
