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
        public List<TaxTags> getTaxTags()
        {
            List<TaxTags> objTaxTags = _context.TaxTags.ToList();
            return objTaxTags;
        }
        public List<Countries> getCountries()
        {
            List<Countries> objCountries = _context.Countries.ToList();
            return objCountries;
        }
        public List<CountryGroups> getCountryGroups()
        {
            List<CountryGroups> objCountryGroups = _context.CountryGroups.ToList();
            return objCountryGroups;
        }
        public List<Contacts> getContacts()
        {
            List<Contacts> objContacts = _context.Contacts.ToList();
            return objContacts;
        }
        public Articles Add(Articles Article)
        {
            return _context.Articles.Add(Article).Entity;
        }
        public int SendNotificationsForArticle(CreateArticleCommand article)
        {
            var publishing = article.EventType == "Publish";

            if (publishing && article.NotificationSentDate == null && article.SendNotification && false)
            {
                var users = ((from uct in (from user in _context.WebsiteUsers
                                           from subscribedCountry in user.UserSubscribedCountries
                                           from subscribedTag in subscribedCountry.UserSubscribedCountryTags.Select(s => s.TaxTag)
                                           select new UserArticleRelation
                                           {
                                               Article = null,
                                               Country = subscribedCountry.Country,
                                               TaxTag = subscribedTag,
                                               WebsiteUser = user
                                           })
                              from act in (from carticle in _context.Articles
                                           from articleTag in carticle.ArticleRelatedTaxTags.Select(at => at.TaxTag)
                                               //from articleCountry in article.ArticleRelatedCountries
                                           let articleCountryGroup = carticle.ArticleRelatedCountryGroups.Select(s => s.CountryGroup).SelectMany(s => s.CountryGroupAssociatedCountries)
                                           let countries = carticle.ArticleRelatedCountries.Select(arc => arc.Country).Concat(articleCountryGroup.Select(g => g.Country))

                                           from country in countries
                                           where
                                               /*article.Type == ArticleType.Article
                                               && */article.IsPublished
                                               && carticle.PublishedDate <= DateTime.UtcNow
                                           select new UserArticleRelation
                                           {
                                               Article = carticle,
                                               Country = country,
                                               TaxTag = articleTag,
                                               WebsiteUser = null
                                           })
                              where
                                   uct.Country.CountryId == act.Country.CountryId
                                   && uct.TaxTag.TaxTagId == act.TaxTag.TaxTagId
                              select new UserArticleRelation
                              {
                                  Article = act.Article,
                                  Country = act.Country,
                                  TaxTag = act.TaxTag,
                                  WebsiteUser = uct.WebsiteUser
                              })
                .Where(r => r.Article.Type == int.Parse(ArticleType.Article.ToString())
                            && r.Article.ArticleId == int.Parse(article.ArticleID))
                .Select(r => r.WebsiteUser)
                .Distinct()
                .Include(r => r.UserDevices)).ToList();
                return users.Count();
            }
            return 0;
        }
        public void Update(Articles order)
        {
            _context.Entry(order).State = EntityState.Modified;
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
        void Update(Articles order);
        Task<Articles> GetAsync(int orderId);
    }
}