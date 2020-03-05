using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class ArticleService : IArticleService
    {
        private readonly TaxathandDbContext _dbContext;
        private readonly ICacheService<Images, ImageDTO> _imageCacheService;
        private readonly ICacheService<Countries, CountryDTO> _countryCacheService;
        private readonly ICommonService _commonService;
        private readonly IUtilityService _utilityService;
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public ArticleService(TaxathandDbContext dbContext,
            ICacheService<Images, ImageDTO> imageCacheService, ICacheService<Countries, CountryDTO> countryCacheService,
            ICommonService commonService, IUtilityService utilityService, ICountryService countryService, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _imageCacheService = imageCacheService ?? throw new ArgumentNullException(nameof(imageCacheService));
            _countryCacheService = countryCacheService ?? throw new ArgumentNullException(nameof(countryCacheService));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ArticleDTO> GetArticleAsync(int articleId, int countryId)
        {
            var locale = "en";
            //var defaultLang = _commonService.GetDefaultLanguageAsync();
            var localeLanguageList = await _commonService.GetLanguageListFromLocale(locale);
            var languageDetails = _commonService.GetLanguageDetails();

            List<int> localeLanguageIdList = new List<int>();
            foreach (var lang in localeLanguageList)
            {
                localeLanguageIdList.Add(lang.LanguageId);
            }

            var article = await GetArticleDetailsAsync(articleId, languageDetails.DefaultLanguageId, localeLanguageIdList,countryId);

            return article;
        }

        private async Task<ArticleDTO> GetArticleDetailsAsync(int articleId, int defaultLanguageId, List<int> localeLanguageIdList,int countryId)
        {
            // Get all images
            var imagesFromCache = await _imageCacheService.GetAllAsync("imagesCacheKey");

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
                                                        a.RelatedCountries
                                                    }).AsNoTracking().ToListAsync();

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

                foreach (var contactrec in contacts)
                {
                    var contact = contacts.FirstOrDefault(c => c.ContactId.Equals(contactrec.ContactId));
                    if (contact != null)
                    {
                        contact.address = addresses.FirstOrDefault(a => a.AddressId.Equals(contact.AddressId));
                        contact.ImagePath = contact.ImageId > 0 ? imagesFromCache.FirstOrDefault(im => im.ImageId.Equals(contact.ImageId)).FilePath : "NA";
                    }
                }
            }

            // Get country data
            var relatedCountries = new CountryResult();
            var relatedCountry = new CountryDTO();
            if (article.RelatedCountries.Count > 0)
            {
                var countryIds = new List<int>();
                countryIds.AddRange(article.RelatedCountries.Select(c => c.IdVal).ToList());

                relatedCountries = await _countryService.GetCountriesByIdsAsync(countryIds, defaultLanguageId, localeLanguageIdList, imagesFromCache);
                relatedCountry = relatedCountries.Countries.FirstOrDefault(rc => rc.Uuid.Equals(countryId));
            }

            var articleDTO = new ArticleDTO
            {
                ContentType = article.Type.Equals(ArticleType.Article) ? Constants.CONTENT_TYPE_ARTICLE : Constants.CONTENT_TYPE_RESOURCE,
                ArticleId = article.ArticleId,
                Title = article.Title,
                TitleInEnglishDefault = article.Title,
                TeaserText = article.TeaserText,
                PublishedDate = Convert.ToString(_commonService.GetUnixEpochTime(Convert.ToDateTime(article.PublishedDate))),
                Author = article.Author,
                ImageCredit = string.Empty,
                ImageDescriptionText = article.ImageId != null ? imagesFromCache.FirstOrDefault(im => im.ImageId.Equals(article.ImageId)).Description : string.Empty,
                Content = $"{_utilityService.FormatArticleContent(article.Content)}",
                RelatedContacts = contacts,
                RelatedCountries = relatedCountries.Countries,
                RelatedCountry = relatedCountry
            };

            return articleDTO;
        }

        public async Task<ILookup<int, Articles>> GetRelatedArticles(IEnumerable<int> articleIds)
        {
            var relatedArticles = await _dbContext.Articles.Where(a => articleIds.Contains(a.ArticleId)).ToListAsync();
            return relatedArticles.ToLookup(r => r.ArticleId);
        }


    }
}