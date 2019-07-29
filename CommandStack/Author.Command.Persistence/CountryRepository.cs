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
        public void Update<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Deleted;
        }
        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
        public void DeleteCountry(Countries country)
        {
            _context.Countries.Remove(country);
        }
        public void DeleteImage(Images image)
        {
            _context.Images.Remove(image);
        }
        public List<Countries> getCountry(List<int> CountryIds)
        {
            List<Countries> objCountry = _context.Countries
                 .Include(s => s.CountryContents)
             .Where(a => CountryIds.Contains(a.CountryId)).ToList();
            return objCountry;
        }
        public List<Images> getImages(List<int?> CountryIds)
        {
            List<Images> objImage = _context.Images
             .Where(a => CountryIds.Contains(a.ImageId)).ToList();
            return objImage;
        }
    }
    public interface ICountryRepository : IRepository<Articles>
    {
        Countries Add(Countries order);
    }
}
