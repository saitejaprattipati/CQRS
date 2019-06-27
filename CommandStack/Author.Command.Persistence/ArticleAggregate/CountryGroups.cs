using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class CountryGroups
    {
        public CountryGroups()
        {
            ArticleRelatedCountryGroups = new HashSet<ArticleRelatedCountryGroups>();
            CountryGroupAssociatedCountries = new HashSet<CountryGroupAssociatedCountries>();
            CountryGroupContents = new HashSet<CountryGroupContents>();
        }

        public int CountryGroupId { get; set; }
        public bool IsPublished { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<ArticleRelatedCountryGroups> ArticleRelatedCountryGroups { get; set; }
        public ICollection<CountryGroupAssociatedCountries> CountryGroupAssociatedCountries { get; set; }
        public ICollection<CountryGroupContents> CountryGroupContents { get; set; }
    }
}
