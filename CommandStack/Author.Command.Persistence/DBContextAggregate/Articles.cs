using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Articles
    {
        public Articles()
        {
            ArticleContents = new HashSet<ArticleContents>();
            ArticleRelatedContacts = new HashSet<ArticleRelatedContacts>();
            ArticleRelatedCountries = new HashSet<ArticleRelatedCountries>();
            ArticleRelatedCountryGroups = new HashSet<ArticleRelatedCountryGroups>();
            ArticleRelatedTaxTags = new HashSet<ArticleRelatedTaxTags>();
            RelatedArticlesArticle = new HashSet<RelatedArticles>();
            RelatedArticlesRelatedArticle = new HashSet<RelatedArticles>();
            RelatedResourcesArticle = new HashSet<RelatedResources>();
            RelatedResourcesRelatedArticle = new HashSet<RelatedResources>();
            UserReadArticles = new HashSet<UserReadArticles>();
            UserSavedArticles = new HashSet<UserSavedArticles>();
        }

        public int ArticleId { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public int? ImageId { get; set; }
        public string State { get; set; }
        public int Type { get; set; }
        public int? SubType { get; set; }
        public int? ResourcePosition { get; set; }
        public int? DisclaimerId { get; set; }
        public int? ResourceGroupId { get; set; }
        public bool IsPublished { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? NotificationSentDate { get; set; }
        public int? ProvinceId { get; set; }

        public virtual Disclaimers Disclaimer { get; set; }
        public virtual Images Image { get; set; }
        public virtual Provinces Province { get; set; }
        public virtual ResourceGroups ResourceGroup { get; set; }
        public virtual ICollection<ArticleContents> ArticleContents { get; set; }
        public virtual ICollection<ArticleRelatedContacts> ArticleRelatedContacts { get; set; }
        public virtual ICollection<ArticleRelatedCountries> ArticleRelatedCountries { get; set; }
        public virtual ICollection<ArticleRelatedCountryGroups> ArticleRelatedCountryGroups { get; set; }
        public virtual ICollection<ArticleRelatedTaxTags> ArticleRelatedTaxTags { get; set; }
        public virtual ICollection<RelatedArticles> RelatedArticlesArticle { get; set; }
        public virtual ICollection<RelatedArticles> RelatedArticlesRelatedArticle { get; set; }
        public virtual ICollection<RelatedResources> RelatedResourcesArticle { get; set; }
        public virtual ICollection<RelatedResources> RelatedResourcesRelatedArticle { get; set; }
        public virtual ICollection<UserReadArticles> UserReadArticles { get; set; }
        public virtual ICollection<UserSavedArticles> UserSavedArticles { get; set; }
    }
}
