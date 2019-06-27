using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class Languages
    {
        public Languages()
        {
            AddressContents = new HashSet<AddressContents>();
            ArticleContents = new HashSet<ArticleContents>();
            ContactContents = new HashSet<ContactContents>();
            CountryContents = new HashSet<CountryContents>();
            CountryGroupContents = new HashSet<CountryGroupContents>();
            DisclaimerContents = new HashSet<DisclaimerContents>();
            ProvinceContents = new HashSet<ProvinceContents>();
            ResourceGroupContents = new HashSet<ResourceGroupContents>();
            TaxTagContents = new HashSet<TaxTagContents>();
            WebsiteUsers = new HashSet<WebsiteUsers>();
        }

        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string NameinEnglish { get; set; }
        public string LocalisationIdentifier { get; set; }
        public string Locale { get; set; }
        public bool RightToLeft { get; set; }

        public ICollection<AddressContents> AddressContents { get; set; }
        public ICollection<ArticleContents> ArticleContents { get; set; }
        public ICollection<ContactContents> ContactContents { get; set; }
        public ICollection<CountryContents> CountryContents { get; set; }
        public ICollection<CountryGroupContents> CountryGroupContents { get; set; }
        public ICollection<DisclaimerContents> DisclaimerContents { get; set; }
        public ICollection<ProvinceContents> ProvinceContents { get; set; }
        public ICollection<ResourceGroupContents> ResourceGroupContents { get; set; }
        public ICollection<TaxTagContents> TaxTagContents { get; set; }
        public ICollection<WebsiteUsers> WebsiteUsers { get; set; }
    }
}
