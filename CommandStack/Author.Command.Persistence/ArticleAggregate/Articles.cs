using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
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

        public Disclaimers Disclaimer { get; set; }
        public Images Image { get; set; }
        public Provinces Province { get; set; }
        public ResourceGroups ResourceGroup { get; set; }
        public ICollection<ArticleContents> ArticleContents { get; set; }
        public ICollection<ArticleRelatedContacts> ArticleRelatedContacts { get; set; }
        public ICollection<ArticleRelatedCountries> ArticleRelatedCountries { get; set; }
        public ICollection<ArticleRelatedCountryGroups> ArticleRelatedCountryGroups { get; set; }
        public ICollection<ArticleRelatedTaxTags> ArticleRelatedTaxTags { get; set; }
        public ICollection<RelatedArticles> RelatedArticlesArticle { get; set; }
        public ICollection<RelatedArticles> RelatedArticlesRelatedArticle { get; set; }
        public ICollection<RelatedResources> RelatedResourcesArticle { get; set; }
        public ICollection<RelatedResources> RelatedResourcesRelatedArticle { get; set; }
        public ICollection<UserReadArticles> UserReadArticles { get; set; }
        public ICollection<UserSavedArticles> UserSavedArticles { get; set; }
    }
}
