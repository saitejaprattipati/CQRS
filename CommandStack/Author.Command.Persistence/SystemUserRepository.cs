using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public bool UserExists(string email, int systemUserId)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;

            // For new user 
            if (systemUserId == 0)
            {
                return _context.SystemUsers.Any(x => email.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase));
            }

            // For existing user
            return _context.SystemUsers.Any(x => (email.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase)) && (x.SystemUserId.Equals(systemUserId)));
        }

        public SystemUsers Add(SystemUsers user)
        {
            return _context.SystemUsers.Add(user).Entity;
        }

        public SystemUserAssociatedCountries Add(SystemUserAssociatedCountries sysuserassociatedcountries)
        {
            return _context.SystemUserAssociatedCountries.Add(sysuserassociatedcountries).Entity;
        }

        public void Update(SystemUsers user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(user).Property(x => x.CreatedBy).IsModified = false;
            _context.Entry(user).Property(x => x.CreatedDate).IsModified = false;
        }

        public async Task<bool> RemoveSystemUserAssociatedCountriesAsync(int systemuserId)
        {
            var existingSysUserCountries = await _context.SystemUserAssociatedCountries.Where(sy => sy.SystemUserId.Equals(systemuserId)).ToListAsync();

            if (existingSysUserCountries.Count == 0)
            {
                return false;
            }

            foreach (var country in existingSysUserCountries)
            {
                _context.SystemUserAssociatedCountries.Remove(country);
            }

            return true;
        }

        public async Task<List<SystemUsers>> GetSystemUsersByIds(List<int> systemuserIds)
        {
            return await _context.SystemUsers.Include(syc => syc.SystemUserAssociatedCountries)
                                            .Where(sy => systemuserIds.Contains<int>(sy.SystemUserId)).ToListAsync();
        }

        public void Delete<T>(T obj)
        {
            _context.Entry(obj).State = EntityState.Deleted;
        }

        public void DeleteSystemUser(SystemUsers systemuser)
        {
            _context.SystemUsers.Remove(systemuser);
        }
    }

    public interface ISystemUserRepository : IRepository<SystemUsers>
    {
        SystemUsers Add(SystemUsers user);

        void Update(SystemUsers user);

        SystemUserAssociatedCountries Add(SystemUserAssociatedCountries sysuserassociatedcountries);

        bool UserExists(string email, int systemUserId);

        Task<bool> RemoveSystemUserAssociatedCountriesAsync(int systemuserId);

        Task<List<SystemUsers>> GetSystemUsersByIds(List<int> systemuserIds);

        void Delete<T>(T obj);
    }
}
