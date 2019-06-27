using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class WebsiteUsers
    {
        public WebsiteUsers()
        {
            UserDevices = new HashSet<UserDevices>();
            UserReadArticles = new HashSet<UserReadArticles>();
            UserSavedArticles = new HashSet<UserSavedArticles>();
            UserSubscribedCountries = new HashSet<UserSubscribedCountries>();
        }

        public int WebsiteUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IndustryName { get; set; }
        public string Location { get; set; }
        public string AuthServiceId { get; set; }
        public Guid CookieId { get; set; }
        public DateTime CookieCreatedDate { get; set; }
        public int AuthServiceName { get; set; }
        public int AuthServiceProviderName { get; set; }
        public int? PreferredLanguageId { get; set; }
        public string UserData { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireTime { get; set; }
        public string DeviceGuid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public Languages PreferredLanguage { get; set; }
        public ICollection<UserDevices> UserDevices { get; set; }
        public ICollection<UserReadArticles> UserReadArticles { get; set; }
        public ICollection<UserSavedArticles> UserSavedArticles { get; set; }
        public ICollection<UserSubscribedCountries> UserSubscribedCountries { get; set; }
    }
}
