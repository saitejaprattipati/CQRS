using Author.Query.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
   public interface IAddressRepository
    {
       // AddressAggregateDetails Get(int pageNo = 1, int pageSize = 100);
       Task<AddressAggregateDetails> Get(int pageNo = 1, int pageSize = 100);
        Task<object> GetById(string id, string partitionKey);
    }
}
