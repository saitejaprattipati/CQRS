using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
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

        public virtual TaxTags ParentTag { get; set; }
        public virtual ICollection<ArticleRelatedTaxTags> ArticleRelatedTaxTags { get; set; }
        public virtual ICollection<TaxTags> InverseParentTag { get; set; }
        public virtual ICollection<TaxTagContents> TaxTagContents { get; set; }
        public virtual ICollection<TaxTagRelatedCountries> TaxTagRelatedCountries { get; set; }
        public virtual ICollection<UserSubscribedCountryTags> UserSubscribedCountryTags { get; set; }
    }
}
