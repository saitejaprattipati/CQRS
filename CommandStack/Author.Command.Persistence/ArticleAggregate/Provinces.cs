using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class Provinces
    {
        public Provinces()
        {
            Articles = new HashSet<Articles>();
            ProvinceContents = new HashSet<ProvinceContents>();
        }

        public int ProvinceId { get; set; }
        public int CountryId { get; set; }

        public Countries Country { get; set; }
        public ICollection<Articles> Articles { get; set; }
        public ICollection<ProvinceContents> ProvinceContents { get; set; }
    }
}
