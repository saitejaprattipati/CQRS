using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain.Models
{
    public class GetArticleById
    {
        public int articleId { get; set; }
        public int countryId { get; set; }
        public string userCookieId { get; set; }
    }
}
