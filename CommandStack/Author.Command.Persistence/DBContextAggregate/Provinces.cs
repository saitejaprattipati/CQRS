using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
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

        public virtual Countries Country { get; set; }
        public virtual ICollection<Articles> Articles { get; set; }
        public virtual ICollection<ProvinceContents> ProvinceContents { get; set; }
    }
}
