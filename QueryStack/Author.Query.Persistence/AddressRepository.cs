using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Author.Core.Services.Persistence.CosmosDB;
using Author.Query.Domain;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Author.Query.Persistence
{
   public class AddressRepository:IAddressRepository
    {
        private readonly CosmosDBContext _context;
        private readonly string addressCollection = "Address";
        private readonly Uri CollectionUri;

        public AddressRepository(IConfiguration configuration)
        {
            _context = new CosmosDBContext();
            CollectionUri = _context.GetCollectionURI(addressCollection);
        }
        public AddressAggregateDetails Get(int pageNo = 1, int pageSize = 100)
        {
            try
            {
                var addressDocs = _context.Get(addressCollection, pageNo, pageSize);
                var addressAggregate = new Collection<AddressAggregate>();
                foreach (var item in addressDocs.Records)
                {
                    var address = AddressAggregate.FromJson(item.ToString());
                    addressAggregate.Add(address);
                }

                AddressAggregateDetails result = new AddressAggregateDetails()
                {
                    Records = addressAggregate.OrderBy(x => x.AddressId)
                };
                if (pageNo > 0 || pageSize > 0)
                {
                    result.PagingResult = new PagingResult
                    {

                        PageNumber = addressDocs.PageNumber,
                        PageSize = addressDocs.PageSize,
                        TotalCount = addressDocs.TotalCount,
                        TotalPages = addressDocs.TotalPages
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
             //   throw new ($"Error occured while fetching", ex);
            }

        }

        public async Task<AddressAggregate> GetById(string id, string partitionKey)
        {
            try
            {
                var address = await _context.Get(partitionKey, addressCollection, id);

                if (address != null)
                {
                    var resourceAggregate = AddressAggregate.FromJson(address.ToString());

                    return resourceAggregate;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                return null;
                //  throw new ($"Error occured while fetching", ex);
            }

        }
    }
}
