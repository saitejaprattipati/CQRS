using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class ArticleService : IArticleService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly ICacheService<Images, ImageDTO> _imageCacheService;
        private readonly ICacheService<Languages, LanguageDTO> _languageCacheService;
        private readonly ICommonService _commonService;
        private readonly IUtilityService _utilityService;
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;
        private readonly IOptions<AppSettings> _appSettings;

        public ArticleService(TaxathandDbContext dbContext,
            ICacheService<Images, ImageDTO> imageCacheService, ICacheService<Languages, LanguageDTO> languageCacheService,
            ICommonService commonService, IUtilityService utilityService, ICountryService countryService, IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _imageCacheService = imageCacheService ?? throw new ArgumentNullException(nameof(imageCacheService));
            _languageCacheService = languageCacheService ?? throw new ArgumentNullException(nameof(languageCacheService));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appSettings = appSettings;
        }
        public async Task<ArticleDTO> GetArticleAsync(int articleId, int countryId, string userCookieId)
        {
            // Get all languages
            var languageFromCache = await _languageCacheService.GetAllAsync("languagesCacheKey");
            //var defaultLanguageId = Convert.ToInt32(_appSettings.Value.DefaultLanguageId);
            //var localeLanguageList = await _commonService.GetLanguageListFromLocale(languageFromCache);
            //var languageDetails = new LanguageDetailsDTO();

            //if ((localeLanguageList.Count == 1) && (localeLanguageList[0].LanguageId.Equals(defaultLanguageId)))
            //{
            //    languageDetails.DefaultLanguageId = localeLanguageList[0].LanguageId;
            //}
            //else
            //{
            //    languageDetails = _commonService.GetLanguageDetails();
            //}
            //var localeLanguageIdList = new List<int>();
            //localeLanguageIdList.AddRange(localeLanguageList.Select(ll => ll.LanguageId));

            //var article = await GetArticleDetailsAsync(articleId, languageDetails.DefaultLanguageId, localeLanguageIdList, countryId, languageFromCache, userCookieId);
            var article = await GetArticleDetailsAsync(articleId, 37, new List<int> { 37}, countryId, languageFromCache, userCookieId);

            return article;
        }

        private async Task<ArticleDTO> GetArticleDetailsAsync(int articleId, int defaultLanguageId, List<int> localeLanguageIdList, int countryId, List<LanguageDTO> languageFromCache, string userCookieId)
        {
            // Get all images
            var imagesFromCache = await _imageCacheService.GetAllAsync("imagesCacheKey");

            // Get all languages
            if (languageFromCache == null)
            {
                languageFromCache = await _languageCacheService.GetAllAsync("languagesCacheKey");
            }

            // Get articles
            var articles = await _dbContext.Articles.Where(a => a.ArticleId.Equals(articleId) && a.IsPublished.Equals(true))
                                                    .OrderByDescending(a => a.ArticleId)
                                                    .Select(a => new
                                                    {
                                                        a.ArticleId,
                                                        a.PublishedDate,
                                                        a.Author,
                                                        a.ImageId,
                                                        a.State,
                                                        a.Type,
                                                        a.SubType,
                                                        a.ResourcePosition,
                                                        a.Disclaimer,
                                                        a.CreatedDate,
                                                        a.UpdatedDate,
                                                        a.NotificationSentDate,
                                                        a.Title,
                                                        a.TeaserText,
                                                        a.Content,
                                                        a.RelatedContacts,
                                                        a.LanguageId,
                                                        a.RelatedArticles,
                                                        a.RelatedResources,
                                                        a.RelatedCountries,
                                                        a.RelatedTaxTags,
                                                        a.ResourceGroup,
                                                        a.Province,
                                                    }).AsNoTracking().ToListAsync();

            // List of available languages for the article
            var availableLanguages = new List<LanguageDTO>();
            if (articles.Count > 0)
            {
                var availablelanguageIds = new List<int>();
                availablelanguageIds.AddRange(articles.Select(a => a.LanguageId).ToList());
                availableLanguages = languageFromCache.Where(lg => availablelanguageIds.Contains(lg.LanguageId)).ToList();
            }

            // Check for preferred local language or else go with defaultLanguageId
            var article = articles.Where(a => localeLanguageIdList.Any(lc => lc.Equals(a.LanguageId)) &&
                                    Convert.ToDateTime(a.PublishedDate) <= DateTime.UtcNow)
                                    .FirstOrDefault();
            if (article == null)
            {
                article = articles.Where(a => a.LanguageId.Equals(defaultLanguageId) &&
                                         Convert.ToDateTime(a.PublishedDate) <= DateTime.UtcNow)
                                         .FirstOrDefault();
            }

            var contacts = new List<ContactDTO>();
            if (article.RelatedContacts.Count > 0)
            {
                var contactIds = new List<int>();
                contactIds.AddRange(article.RelatedContacts.Select(c => c.IdVal).ToList());

                contacts = await _dbContext.Contacts.Where(co => contactIds.Contains(co.ContactId))
                                    .ProjectTo<ContactDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();


                var addresses = await _dbContext.Address.Where(ad => contacts.Select(c => c.AddressId).Any(c => c.Equals(ad.AddressId)))
                                                        .ProjectTo<AddressDTO>(_mapper.ConfigurationProvider)
                                                        .AsNoTracking()
                                                        .ToListAsync();

                //foreach (var contactrec in contacts)
                //{
                //    var contact = contacts.FirstOrDefault(c => c.ContactId.Equals(contactrec.ContactId));
                //    if (contact != null)
                //    {
                //        contact.address = addresses.FirstOrDefault(a => a.AddressId.Equals(contact.AddressId));
                //        contact.ImagePath = contact.ImageId > 0 ? imagesFromCache.FirstOrDefault(im => im.ImageId.Equals(contact.ImageId)).FilePath : "NA";
                //    }
                //}

                contacts = await _dbContext.Contacts.Where(co => contactIds.Contains(co.ContactId)).ProjectTo<ContactDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();

                Parallel.ForEach(contacts, contact => Update(contact, addresses, imagesFromCache));
            }

            // Get country data
            var relatedCountries = new List<CountryDTO>();
            var relatedCountry = new CountryDTO();
            var countryResult = await _countryService.GetAllCountriesAsync();
            if (article.RelatedCountries.Count > 0)
            {
                var countryIds = new List<int>();
                countryIds.AddRange(article.RelatedCountries.Select(c => c.IdVal).ToList());
                relatedCountries = countryResult.Countries.Where(c => countryIds.Contains(c.Uuid)).ToList();
                ////relatedCountries = await _countryService.GetCountriesByIdsAsync(countryIds, defaultLanguageId, localeLanguageIdList, imagesFromCache);
                relatedCountry = relatedCountries.FirstOrDefault(rc => rc.Uuid.Equals(countryId));
            }

            // Get Article Disclaimer
            var disClaimerDetails = (article.Disclaimer != null) ? Tuple.Create(article.Disclaimer.DisclaimerId,
                                                                                    article.Disclaimer.ProviderName,
                                                                                    article.Disclaimer.ProviderTerms) : null;

            // Get image details
            Tuple<int, string, string> imageDetails = null;

            var image = imagesFromCache.FirstOrDefault(im => im.ImageId.Equals(article.ImageId));
            imageDetails = new Tuple<int, string, string>(image.ImageId, image.FilePath, image.Description);

            // Get ResourceGroup details
            var resourceGroup = _mapper.Map<ResourceGroupDTO>(article.ResourceGroup) ?? null;

            // Get Province details
            var province = _mapper.Map<ProvinceDTO>(article.Province) ?? null;

            // Get tags associated with Articles
            var articleRelatedtags = (article.RelatedTaxTags.Count > 0) ? _mapper.Map<ICollection<RelatedTaxTagsSchema>, List<TaxTagsDTO>>(article.RelatedTaxTags) : null;

            // Get RelatedArticles
            var relatedArticles = (article.RelatedArticles.Count() > 0) ? GetRelatedArtifactAssociatedWithArticle(article.RelatedArticles, countryResult.Countries) : null;

            // Get RelatedResources
            var relatedResources = (article.RelatedResources.Count() > 0) ? GetRelatedArtifactAssociatedWithArticle(article.RelatedResources, countryResult.Countries) : null;

            // Get details based on userCookieId
            Tuple<bool, bool, string> userActivityDetails = null;
            var relatedSubcriptions = new List<SubscriptionDTO>();
            if (!string.IsNullOrWhiteSpace(userCookieId))
            {
                var user = await _dbContext.WebsiteUsers.AsNoTracking().FirstOrDefaultAsync(u => u.CookieId.Equals(userCookieId));

                var userActivities = await _dbContext.UserActivities.AsNoTracking().ToListAsync();

                var userActivity = userActivities.FirstOrDefault(ua => ua.WebsiteUserId.Equals(user.WebsiteUserId));

                var isRead = userActivity.ReadArticles != null ? userActivity.ReadArticles.Any(ur => ur.ArticleId.Equals(articleId)) : false;

                var savedDate = userActivity.SavedArticles != null ? userActivity.SavedArticles.FirstOrDefault(ua => ua.ArticleId.Equals(articleId)) : new UserSavedArticles();

                userActivityDetails = new Tuple<bool, bool, string>(isRead, (savedDate != null ? true : false),
                                          savedDate.UpdatedDate);

                // Get relatedsubscriptions
                var subscribedCountries = (userActivity.SubscribedCountries != null) ?
                                          userActivity.SubscribedCountries.Select(ua => ua.Country).ToList() :
                                          new List<IEnumerable<SubscribedCountryTags>>();

                foreach (var subscription in subscribedCountries)
                {
                    foreach (var item in subscription)
                    {
                        if (relatedCountries.Any(rc => rc.Uuid.Equals(item.CountryId)))
                        {
                            if (articleRelatedtags.Any(rt => rt.TaxTagId.Equals(item.TaxTagId)))
                            {
                                relatedSubcriptions.Add(new SubscriptionDTO { CountryUUID = item.CountryId, TagUUID = item.TaxTagId, IsSubscribed = true });
                            }
                        }
                    }
                }
            }

            var articleDTO = new ArticleDTO
            {
                ContentType = (article.Type.Equals((int)ArticleType.Article)) ? Constants.CONTENT_TYPE_ARTICLE : Constants.CONTENT_TYPE_RESOURCE,
                ArticleId = article.ArticleId,
                Title = article.Title,
                TitleInEnglishDefault = article.Title,
                TeaserText = article.TeaserText,
                PublishedDate = Convert.ToString(_commonService.GetUnixEpochTime(Convert.ToDateTime(article.PublishedDate))),
                Author = article.Author,
                ImageCredit = string.Empty,
                ImageDescriptionText = imageDetails.Item3 ?? string.Empty,
                ImagePath = imageDetails.Item2 ?? string.Empty,
                Content = $"{_utilityService.FormatArticleContent(article.Content)}<p>{disClaimerDetails.Item2 ?? string.Empty}</p><p>{disClaimerDetails.Item3 ?? string.Empty}</p>",
                RelatedContacts = contacts,
                RelatedCountries = relatedCountries,
                RelatedCountry = relatedCountry,
                RelatedTaxTags = articleRelatedtags,
                ResourcePosition = article.ResourcePosition,
                ResourceGroup = resourceGroup,
                Province = province.DisplayName,
                AvailableLanguages = availableLanguages,
                ContainsYoutubeLink = _utilityService.IsContainYouTubeLinks(article.Content),
                RelatedArticles = relatedArticles,
                RelatedResources = relatedResources,
                IsRead = (!string.IsNullOrWhiteSpace(userCookieId)) ? userActivityDetails.Item1 : false,
                Saved = (!string.IsNullOrWhiteSpace(userCookieId)) ? userActivityDetails.Item2 : false,
                SavedDate = (!string.IsNullOrWhiteSpace(userCookieId)) ? userActivityDetails.Item3 : string.Empty,
                RelatedSubscriptions = relatedSubcriptions
            };

            return articleDTO;
        }

        private List<RelatedArticleDTO> GetRelatedArtifactAssociatedWithArticle(IEnumerable<RelatedArticlesSchema> relatedArticlesList, List<CountryDTO> countries)
        {
            var result = new List<CountryDTO>();
            var relatedArticles = new List<RelatedArticleDTO>();
            foreach (var rc in relatedArticlesList)
            {
                var relatedCountryIds = new List<int>();
                relatedCountryIds.AddRange(rc.RelatedCountries.Select(r => r.IdVal).ToList());
                result = countries.Where(rc => relatedCountryIds.Contains(rc.Uuid)).ToList();
                relatedArticles.Add(new RelatedArticleDTO
                {
                    ArticleId = rc.ArticleId,
                    RelatedCountries = result,
                    PublishedDate = rc.PublishedDate,
                    Title = rc.Title
                });
            }
            return relatedArticles;
        }

        private void Update(ContactDTO contact, List<AddressDTO> addresses, List<ImageDTO> imagesFromCache)
        {
            contact.address = addresses.FirstOrDefault(a => a.AddressId.Equals(contact.AddressId));
            contact.ImagePath = contact.ImageId > 0 ? imagesFromCache.FirstOrDefault(im => im.ImageId.Equals(contact.ImageId)).FilePath : "NA";
        }

        public async Task<ILookup<int, Articles>> GetRelatedArticles(IEnumerable<int> articleIds)
        {
            var relatedArticles = await _dbContext.Articles.Where(a => articleIds.Contains(a.ArticleId)).ToListAsync();
            return relatedArticles.ToLookup(r => r.ArticleId);
        }
    }
}