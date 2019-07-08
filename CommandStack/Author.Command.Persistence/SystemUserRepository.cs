using Author.Command.Persistence.DBContextAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Author.Command.Persistence
{
    public class SystemUserRepository : ISystemUserRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public SystemUserRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool UserExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return true;

            return _context.SystemUsers.Any(x => email.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase));
        }

        public SystemUsers Add(SystemUsers user)
        {
            return _context.SystemUsers.Add(user).Entity;
        }

        public SystemUserAssociatedCountries Add(SystemUserAssociatedCountries sysuserassociatedcountries)
        {
            return _context.SystemUserAssociatedCountries.Add(sysuserassociatedcountries).Entity;
        }
    }

   public interface ISystemUserRepository : IRepository<SystemUsers>
    {
        SystemUsers Add(SystemUsers user);

        SystemUserAssociatedCountries Add(SystemUserAssociatedCountries sysuserassociatedcountries);

        bool UserExists(string email);
    }
}
