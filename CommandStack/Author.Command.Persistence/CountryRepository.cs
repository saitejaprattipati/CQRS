using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
   public class CountryRepository : ICountryRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public CountryRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Countries Add(Countries _countries)
        {
            return _context.Countries.Add(_countries).Entity;
        }
        public Images AddImage(Images _images)
        {
            return _context.Images.Add(_images).Entity;
        }
        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
    }
    public interface ICountryRepository : IRepository<Articles>
    {
        Countries Add(Countries order);
    }
}
