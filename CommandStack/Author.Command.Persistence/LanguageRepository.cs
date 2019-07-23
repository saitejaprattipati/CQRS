using Author.Command.Persistence.DBContextAggregate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Author.Command.Persistence
{
    public class LanguageRepository: ILanguageRepository
    {
        private readonly TaxatHand_StgContext _context;

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
