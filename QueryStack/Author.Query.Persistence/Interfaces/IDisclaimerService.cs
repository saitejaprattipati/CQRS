using Author.Query.Persistence.DTO;
using System.Threading.Tasks;

namespace Author.Query.Persistence.Interfaces
{
    public interface IDisclaimerService
    {
        Task<DisclaimerResult> GetAllDisclaimersAsync();

        Task<DisclaimerDTO> GetDiscalimerAsync(int disclaimerId);
    }
}
