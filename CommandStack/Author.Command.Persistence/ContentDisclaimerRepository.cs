using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Command.Persistence
{
    public class ContentDisclaimerRepository: IContentDisclaimerRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public ContentDisclaimerRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Disclaimers> GetContentDisclaimer(int disclaimerId)
        {
            //Update existing disclaimer
            return await _context.Disclaimers.Include(d => d.DisclaimerContents)
                                                  .Where(dc => dc.DisclaimerId.Equals(disclaimerId))
                                                  .FirstOrDefaultAsync();
        }


        public async Task<Disclaimers> AddAsync(Disclaimers contentDisclaimer)
        {
            await _context.Disclaimers.AddAsync(contentDisclaimer);
            return contentDisclaimer;
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

    public interface IContentDisclaimerRepository : IRepository<Disclaimers>
    {
        Task<Disclaimers> AddAsync(Disclaimers contentDisclaimer);
        void Update<T>(T obj);
        void Delete<T>(T obj);
    }
}
