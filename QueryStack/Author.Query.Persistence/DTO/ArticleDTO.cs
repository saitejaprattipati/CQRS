using Author.Core.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class ArticlesResult
    {
        public List<ArticleDTO> Articles { get; set; }

        public ArticlesResult()
        {
            Articles = new List<ArticleDTO>();
        }
    }

    public class ArticleDTO
    {
        public int ArticleId { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string PublishedDate { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Author { get; set; }
        public int? ResourcePosition { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Province { get; set; }
        public int LanguageId { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Title { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string TeaserText { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ContentType { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Content { get; set; }
        public List<ContactDTO> RelatedContacts { get; set; }
        public List<CountryDTO> RelatedCountries { get; set; }
        public CountryDTO RelatedCountry { get; set; }
        public List<TaxTagsDTO> RelatedTaxTags { get; set; }
        public List<ArticleDTO> RelatedArticles { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ImageCredit { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ImageDescriptionText { get; set; }
        //public List<Locale> AvailableLanguages { get; set; }
        public bool IsRead { get; set; }
        public bool Saved { get; set; }
        public long SavedDate { get; set; }
        //public List<Subscription> RelatedSubscriptions { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string GroupName { get; set; }
        public int? GroupPosition { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string SharingWebURL { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ImagePath { get; set; }
        public bool Subscribed { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Name { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Path { get; set; }
        public bool CompleteResponse { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string TitleInEnglishDefault { get; set; }
        public bool ContainsYoutubeLink { get; set; }
        //public ArticleDTO()
        //{
        //    RelatedContent = new List<ArticleDTO>();
        //    RelatedContacts = new List<ContactDTO>();
        //    RelatedTags = new List<RelatedTag>();
        //    AvailableLanguages = new List<Locale>();
        //    RelatedSubscriptions = new List<Subscription>();
        //}
    }
}
