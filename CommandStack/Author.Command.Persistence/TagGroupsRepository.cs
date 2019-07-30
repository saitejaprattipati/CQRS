using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
    public class TagGroupsRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public TagGroupsRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public TaxTags Add(TaxTags _taxTags)
        {
            return _context.TaxTags.Add(_taxTags).Entity;
        }
        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
        public List<TaxTags> GetTagGroups(List<int> taxGroupsIds)
        {
            List<TaxTags> objtaggroups = _context.TaxTags
                 .Include(s => s.TaxTagContents)
                 .Include(s => s.TaxTagRelatedCountries)
             .Where(a => taxGroupsIds.Contains(a.TaxTagId)).ToList();
            return objtaggroups;
        }
        public void Update<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Deleted;
        }
        public void DeletetagGroup(TaxTags tagGroup)
        {
            _context.TaxTags.Remove(tagGroup);
        }
    }
}
