using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Countries
    {
        public Countries()
        {
            ArticleRelatedCountries = new HashSet<ArticleRelatedCountries>();
            Contacts = new HashSet<Contacts>();
            CountryContents = new HashSet<CountryContents>();
            CountryGroupAssociatedCountries = new HashSet<CountryGroupAssociatedCountries>();
            Images = new HashSet<Images>();
            Provinces = new HashSet<Provinces>();
            SystemUserAssociatedCountries = new HashSet<SystemUserAssociatedCountries>();
            TaxTagRelatedCountries = new HashSet<TaxTagRelatedCountries>();
            UserSubscribedCountries = new HashSet<UserSubscribedCountries>();
        }

        public int CountryId { get; set; }
        public int? SvgimageId { get; set; }
        public int? PngimageId { get; set; }
        public bool IsPublished { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Images Pngimage { get; set; }
        public virtual Images Svgimage { get; set; }
        public virtual ICollection<ArticleRelatedCountries> ArticleRelatedCountries { get; set; }
        public virtual ICollection<Contacts> Contacts { get; set; }
        public virtual ICollection<CountryContents> CountryContents { get; set; }
        public virtual ICollection<CountryGroupAssociatedCountries> CountryGroupAssociatedCountries { get; set; }
        public virtual ICollection<Images> Images { get; set; }
        public virtual ICollection<Provinces> Provinces { get; set; }
        public virtual ICollection<SystemUserAssociatedCountries> SystemUserAssociatedCountries { get; set; }
        public virtual ICollection<TaxTagRelatedCountries> TaxTagRelatedCountries { get; set; }
        public virtual ICollection<UserSubscribedCountries> UserSubscribedCountries { get; set; }
    }
}
