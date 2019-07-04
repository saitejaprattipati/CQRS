using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public Articles Add(Articles Article)
        {
            //var obj = _context.Images
            //    .Include(c=>c.Articles)
            //    .ThenInclude(a=>a.ArticleContents)
            //    .ToList();
            //var obj1 = _context.Images
            //    .ToList();
            return _context.Articles.Add(Article).Entity;

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