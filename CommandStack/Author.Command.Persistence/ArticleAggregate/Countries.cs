using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
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

        public Images Pngimage { get; set; }
        public Images Svgimage { get; set; }
        public ICollection<ArticleRelatedCountries> ArticleRelatedCountries { get; set; }
        public ICollection<Contacts> Contacts { get; set; }
        public ICollection<CountryContents> CountryContents { get; set; }
        public ICollection<CountryGroupAssociatedCountries> CountryGroupAssociatedCountries { get; set; }
        public ICollection<Images> Images { get; set; }
        public ICollection<Provinces> Provinces { get; set; }
        public ICollection<TaxTagRelatedCountries> TaxTagRelatedCountries { get; set; }
        public ICollection<UserSubscribedCountries> UserSubscribedCountries { get; set; }
    }
}
