using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Images
    {
        public Images()
        {
            Articles = new HashSet<Articles>();
            Contacts = new HashSet<Contacts>();
            CountriesPngimage = new HashSet<Countries>();
            CountriesSvgimage = new HashSet<Countries>();
        }

        public int ImageId { get; set; }
        public string Name { get; set; }
        public int ImageType { get; set; }
        public int? CountryId { get; set; }
        public string Keyword { get; set; }
        public string Source { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string EmpGuid { get; set; }
        public bool Edited { get; set; }

        public virtual Countries Country { get; set; }
        public virtual ICollection<Articles> Articles { get; set; }
        public virtual ICollection<Contacts> Contacts { get; set; }
        public virtual ICollection<Countries> CountriesPngimage { get; set; }
        public virtual ICollection<Countries> CountriesSvgimage { get; set; }
    }
}
