using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
    public class SiteDisclaimerRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public SiteDisclaimerRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Articles> GetSiteDisclaimer(int siteDisclaimerId)
        {
            //Update existing disclaimer
            return await _context.Articles.Include(a => a.ArticleContents)
                                                  .Where(a => a.ArticleId.Equals(siteDisclaimerId))
                                                  .FirstOrDefaultAsync(a => a.Type == Convert.ToInt32(ArticleType.Page));
        }
        public Disclaimers getDisclaimerById(int DisclaimerId)
        {
            Disclaimers objContact = _context.Disclaimers.Include(s => s.DisclaimerContents).Where(d => d.DisclaimerId == DisclaimerId).FirstOrDefault();
            return objContact;
        }
        public async Task<List<Articles>> GetSiteDisclaimerByIds(List<int> siteDisclaimerIds)
        {
            var pageType = Convert.ToInt32(ArticleType.Page);
            return await _context.Articles.Include(ac => ac.ArticleContents)
                                          .Include(arc=>arc.ArticleRelatedCountries)
                                          .Include(p => p.ArticleRelatedCountryGroups)
                                          .Include(h => h.ArticleRelatedTaxTags)
                                          .Include(k => k.RelatedArticlesArticle)
                                          .Include(l => l.RelatedResourcesArticle)
                                          .Include(m => m.UserReadArticles)
                                          .Include(n => n.UserSavedArticles)
                                          .Include(o => o.ArticleRelatedContacts)
                                          .Where(s => siteDisclaimerIds.Contains<int>(s.ArticleId) && s.Type.Equals(pageType)).ToListAsync();
        }

        public Articles Add(Articles article)
        {
            return _context.Articles.Add(article).Entity;
        }

        public async Task<Articles> AddAsync(Articles article)
        {
            await _context.Articles.AddAsync(article);
            return article;
        }


        public void Update<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Deleted;
        }

        public void DeleteRelatedArticles(int relatedArticleId)
        {
            _context.RelatedArticles.RemoveRange(_context.RelatedArticles.Where(ra => ra.RelatedArticleId.Equals(relatedArticleId)));
        }

        public void DeleteRelatedResources(int relatedResourceId)
        {
            _context.RelatedResources.RemoveRange(_context.RelatedResources.Where(rr=>rr.RelatedArticleId.Equals(relatedResourceId)));
        }
    }

    public interface ISiteDisclaimerRepository : IRepository<Articles>
    {
        Articles GetSiteDisclaimer(int siteDisclaimerId);

        Task<List<Articles>> GetSiteDisclaimerByIds(List<int> siteDisclaimerIds);

        Articles Add(Articles article);

        Task<Articles> AddAsync(Articles article);

        void DeleteRelatedArticles(int relatedArticleId);

        void DeleteRelatedResources(int relatedResourceId);
    }
}
