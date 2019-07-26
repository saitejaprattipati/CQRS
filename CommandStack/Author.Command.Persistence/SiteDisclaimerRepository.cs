using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Microsoft.EntityFrameworkCore;
using System;
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
    }

    public interface ISiteDisclaimerRepository : IRepository<Articles>
    {
        Articles GetSiteDisclaimer(int siteDisclaimerId);

        Articles Add(Articles article);

        Task<Articles> AddAsync(Articles article);
    } 
}
