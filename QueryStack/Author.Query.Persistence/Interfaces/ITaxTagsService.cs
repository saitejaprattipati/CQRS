using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface ITaxTagsService
    {
        Task<TaxTagGroupsResult> GetTaxTagGroupsAsync(LanguageDTO language);

        Task<TaxTagGroupDTO> GetTaxTagGroupAsync(LanguageDTO language, int taxTagId);
    }
}
