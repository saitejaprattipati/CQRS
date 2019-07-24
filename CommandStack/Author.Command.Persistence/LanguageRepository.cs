using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Author.Command.Persistence
{
    public class LanguageRepository: ILanguageRepository
    {
        private readonly TaxatHand_StgContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public LanguageRepository(TaxatHand_StgContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Languages> GetAllLanguages()
        {
            var languages = _context.Languages.ToList();
            return _context.Languages.AsNoTracking().ToList();
        }
    }

    public interface ILanguageRepository
    {
        List<Languages> GetAllLanguages();
    }
}
