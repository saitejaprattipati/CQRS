using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Core.Framework;

namespace Author.Command.Persistence
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly TaxatHand_StgContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public ArticleRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public List<Articles> getArticles()
        {
            List<Articles> objArticles = _context.Articles.ToList();
            return objArticles;
        }
        public List<Articles> getArticlesListById(List<int> ArticleIds)
        {
            List<Articles> objArticles = _context.Articles.Where(a => ArticleIds.Contains(a.ArticleId)).ToList();//   _context.Articles.ToList();
            return objArticles;
        }
        public List<Articles> getArticleCompleteDataById(List<int> ArticleIds)
        {
            List<Articles> objArticles=new List<Articles>();// = _context.Articles.ToList();

            foreach(var article in ArticleIds)
            {
                objArticles.Add(_context.Articles
                    .Include(a => a.ArticleContents)
                    .Include(a => a.ArticleRelatedCountries)//.Select(rc => rc.CountryContent))
                    .Include(a => a.ArticleRelatedCountryGroups)//.Select(rcg => rcg.CountryGroupContent))
                    .Include(a => a.ArticleRelatedTaxTags)//.Select(rc => rc.TaxTagContent.Select(tl => tl.Language)))
                    .Include(a => a.ArticleRelatedContacts)//.Select(rc => rc.ContactContent))
                    .Include(a => a.RelatedArticlesArticle)//.Select(ra => ra.ArticleContent))
                    .Include(a => a.RelatedResourcesArticle)//.Select(ra => ra.ArticleContent))
                    .Include(a => a.UserReadArticles)
                    .Include(a => a.UserSavedArticles)
                    // .Include(a => a.Disclaimer.DisclaimerContent)
                    .FirstOrDefault(a => a.ArticleId == article));
            }
            return objArticles;
        }
        public Articles getArticleDataById(int ArticleId)
        {
            Articles _article = _context.Articles.FirstOrDefault(a => a.ArticleId == ArticleId);
            return _article;
        }
        public List<TaxTags> getTaxTags()
        {
            List<TaxTags> objTaxTags = _context.TaxTags.ToList();
            return objTaxTags;
        }
        public void DeleteArticle(Articles article)
        {
            _context.Articles.Remove(article);
        }
        public TaxTags getTaxTagsById(int TaxTagsId)
        {
            TaxTags objTaxTags = _context.TaxTags.Where(s=>s.TaxTagId== TaxTagsId).FirstOrDefault();
            return objTaxTags;
        }
        public List<TaxTags> getTaxTagsDetailsByIds(List<int> TaxTagsId)
        {
            List<TaxTags> objTaxTags = _context.TaxTags.Include(s => s.TaxTagContents).Where(s => TaxTagsId.Contains(s.TaxTagId)).ToList();
            return objTaxTags;
        }
        public List<Countries> getCountriesByIds(List<int> CountryIds)
        {
            List<Countries> objCountries = _context.Countries.Where(s => CountryIds.Contains(s.CountryId)).ToList();
            return objCountries;
        }
        public CountryGroups getCountryGroupById(int CountryGroupId)
        {
            CountryGroups objCountryGroup = _context.CountryGroups.Where(s=>s.CountryGroupId == CountryGroupId).FirstOrDefault();
            return objCountryGroup;
        }
        public Countries getCountryById(int CountryId)
        {
            Countries objCountries = _context.Countries.Where(s=>s.CountryId==CountryId).FirstOrDefault();
            return objCountries;
        }
        public List<CountryGroups> getCountryGroupsByIds(List<int> CountryGroupIds)
        {
            List<CountryGroups> objCountryGroups = _context.CountryGroups.Where(s => CountryGroupIds.Contains(s.CountryGroupId)).ToList();
            return objCountryGroups;
        }
        public List<Contacts> getContactsByIds(List<int> ContactIds)
        {
            List<Contacts> objContacts = _context.Contacts.Where(s => ContactIds.Contains(s.ContactId)).ToList();
            return objContacts;
        }
        public Contacts getContactsById(int ContactId)
        {
            Contacts objContact = _context.Contacts.Where(s=>s.ContactId== ContactId).FirstOrDefault();
            return objContact;
        }
        public Disclaimers getDisclaimerById(int DisclaimerId)
        {
            Disclaimers objContact = _context.Disclaimers.Include(s=>s.DisclaimerContents).Where(d => d.DisclaimerId == DisclaimerId).FirstOrDefault();
            return objContact;
        }
        public ResourceGroups getResourceGroupById(int ResourceGroupId)
        {
            ResourceGroups objContact = _context.ResourceGroups.Include(s=>s.ResourceGroupContents).Where(r => r.ResourceGroupId == ResourceGroupId).FirstOrDefault();
            return objContact;
        }
        public Provinces getProvisionsById(int ProvisionsId)
        {
            Provinces objContact = _context.Provinces.Include(s=>s.ProvinceContents).Where(p => p.ProvinceId == ProvisionsId).FirstOrDefault();
            return objContact;
        }
        public Articles Add(Articles Article)
        {
            return _context.Articles.Add(Article).Entity;
        }
        public int SendNotificationsForArticle<T>(T article)
        {
           // var publishing = article.EventType == "Publish";

            //if (publishing && article.NotificationSentDate == null && article.SendNotification && false)
            //{
            //    var users = ((from uct in (from user in _context.WebsiteUsers
            //                               from subscribedCountry in user.UserSubscribedCountries
            //                               from subscribedTag in subscribedCountry.UserSubscribedCountryTags.Select(s => s.TaxTag)
            //                               select new UserArticleRelation
            //                               {
            //                                   Article = null,
            //                                   Country = subscribedCountry.Country,
            //                                   TaxTag = subscribedTag,
            //                                   WebsiteUser = user
            //                               })
            //                  from act in (from carticle in _context.Articles
            //                               from articleTag in carticle.ArticleRelatedTaxTags.Select(at => at.TaxTag)
            //                                   //from articleCountry in article.ArticleRelatedCountries
            //                               let articleCountryGroup = carticle.ArticleRelatedCountryGroups.Select(s => s.CountryGroup).SelectMany(s => s.CountryGroupAssociatedCountries)
            //                               let countries = carticle.ArticleRelatedCountries.Select(arc => arc.Country).Concat(articleCountryGroup.Select(g => g.Country))

            //                               from country in countries
            //                               where
            //                                   /*article.Type == ArticleType.Article
            //                                   && */article.IsPublished
            //                                   && carticle.PublishedDate <= DateTime.UtcNow
            //                               select new UserArticleRelation
            //                               {
            //                                   Article = carticle,
            //                                   Country = country,
            //                                   TaxTag = articleTag,
            //                                   WebsiteUser = null
            //                               })
            //                  where
            //                       uct.Country.CountryId == act.Country.CountryId
            //                       && uct.TaxTag.TaxTagId == act.TaxTag.TaxTagId
            //                  select new UserArticleRelation
            //                  {
            //                      Article = act.Article,
            //                      Country = act.Country,
            //                      TaxTag = act.TaxTag,
            //                      WebsiteUser = uct.WebsiteUser
            //                  })
            //    .Where(r => r.Article.Type == int.Parse(ArticleType.Article.ToString())
            //                && r.Article.ArticleId == int.Parse(article.ArticleID))
            //    .Select(r => r.WebsiteUser)
            //    .Distinct()
            //    .Include(r => r.UserDevices)).ToList();
            //    return users.Count();
            //}
            return 0;
        }
        public void Update<T>(T order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }
        public void Delete<T>(T order)
        {
            _context.Entry(order).State = EntityState.Deleted;
        }
        public async Task<Articles> GetAsync(int orderId)
        {
            var order = await _context.Articles.FindAsync(orderId);

            return order;
        }
    }
    public interface IArticleRepository : IRepository<Articles>
    {
        Articles Add(Articles order);
        void Update<T>(T order);
        Task<Articles> GetAsync(int orderId);
    }
}