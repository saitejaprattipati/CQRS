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
            return _context.ResourceGroups.Add(_ResourceGroup).Entity;
        }
        public List<ResourceGroups> getResourceGroups(List<int> ResourceGroupIds)
        {
            List<ResourceGroups> objresourcegroups = _context.ResourceGroups
                 .Include(s => s.ResourceGroupContents)
             .Where(a => ResourceGroupIds.Contains(a.ResourceGroupId)).ToList();
            return objresourcegroups;
        }
        public ResourceGroups UnPublishResourceGroups(ResourceGroups _ResourceGroup)
        {
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
        public void DeleteResourceGroup(ResourceGroups resourceGroup)
        {
            _context.ResourceGroups.Remove(resourceGroup);
        }
        //public ResourceGroups GetResourceGroup(int resourceGroupId)
        //{
        //    var ResourceGroup = (from rg in _context.ResourceGroups
        //                        .Include(c => c.ResourceGroupContents)
        //                         .Where(b => b.ResourceGroupId == resourceGroupId)
        //                         select (rg)).FirstOrDefault();
        //    return ResourceGroup;
        //}
        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
    }
    public interface IResourceGroupRepository : IRepository<Articles>
    {
        ResourceGroups Add(ResourceGroups order);
    }
}
