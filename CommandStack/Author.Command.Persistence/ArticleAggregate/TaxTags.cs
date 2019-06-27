using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class TaxTags
    {
        public TaxTags()
        {
            ArticleRelatedTaxTags = new HashSet<ArticleRelatedTaxTags>();
            InverseParentTag = new HashSet<TaxTags>();
            TaxTagContents = new HashSet<TaxTagContents>();
            TaxTagRelatedCountries = new HashSet<TaxTagRelatedCountries>();
            UserSubscribedCountryTags = new HashSet<UserSubscribedCountryTags>();
        }

        public int TaxTagId { get; set; }
        public int? ParentTagId { get; set; }
        public bool IsPublished { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public TaxTags ParentTag { get; set; }
        public ICollection<ArticleRelatedTaxTags> ArticleRelatedTaxTags { get; set; }
        public ICollection<TaxTags> InverseParentTag { get; set; }
        public ICollection<TaxTagContents> TaxTagContents { get; set; }
        public ICollection<TaxTagRelatedCountries> TaxTagRelatedCountries { get; set; }
        public ICollection<UserSubscribedCountryTags> UserSubscribedCountryTags { get; set; }
    }
}
