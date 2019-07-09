using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
   public class ResourceGroupRepository : IResourceGroupRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ResourceGroupRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ResourceGroups Add(ResourceGroups _ResourceGroup)
        {
            //var obj = _context.Images
            //    .Include(c=>c.Articles)
            //    .ThenInclude(a=>a.ArticleContents)
            //    .ToList();
            //var obj1 = _context.Images
            //    .ToList();
            return _context.ResourceGroups.Add(_ResourceGroup).Entity;

        }

        public void Update<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Deleted;
        }


        public ResourceGroups GetResourceGroup(int resourceGroupId)
        {
            var ResourceGroup = (from rg in _context.ResourceGroups
                                .Include(c=>c.ResourceGroupContents)                                
                                 .Where(b => b.ResourceGroupId == resourceGroupId)
                        select(rg)).FirstOrDefault();

            return ResourceGroup;
        }
        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
    }
    public class RecordsBase<T>
    {

        public IEnumerable<T> Records { get; set; }
    }
    public interface IResourceGroupRepository : IRepository<Articles>
    {
        ResourceGroups Add(ResourceGroups order);

        //void Update(Articles order);

        //Task<Articles> GetAsync(int orderId);
    }
}
