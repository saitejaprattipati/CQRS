using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
   public class CountryGroupsRepository : ICountryGroupsRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public CountryGroupsRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public CountryGroups Add(CountryGroups _countryGroups)
        {
            return _context.CountryGroups.Add(_countryGroups).Entity;
        }
        public List<CountryGroups> getCountryGroups(List<int> CountryGroupIds)
        {
            List<CountryGroups> objCountrygroups = _context.CountryGroups
                 .Include(s => s.CountryGroupContents)
                  .Include(s => s.CountryGroupAssociatedCountries)
             .Where(a => CountryGroupIds.Contains(a.CountryGroupId)).ToList();
            return objCountrygroups;
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
    public interface ICountryGroupsRepository : IRepository<Articles>
    {
        CountryGroups Add(CountryGroups order);
    }
}
