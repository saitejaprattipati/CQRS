using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface IDisclaimerService
    {
        Task<DisclaimerResult> GetAllDisclaimersAsync(LanguageDTO language, int pageNo, int pageSize);

        Task<DisclaimerDTO> GetDiscalimerAsync(LanguageDTO language, int disclaimerId);
    }
}
