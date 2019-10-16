using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Author.Core.Services.Persistence.CosmosDB;
using Author.Query.Domain;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Author.Query.Domain.DBAggregate;

namespace Author.Query.Persistence
{
    public class AddressRepository : IAddressRepository
    {
        private readonly CosmosDBContext _context;
        private readonly string addressCollection = "Address";
        private readonly Uri CollectionUri;
        private readonly TaxathandDbContext _dbContext;
        public AddressRepository(IConfiguration configuration, TaxathandDbContext dbContext)
        {
            _dbContext = dbContext;
            //_context = new CosmosDBContext();
            //  CollectionUri = _context.GetCollectionURI(addressCollection);
        }
        //public AddressAggregateDetails Get(int pageNo = 1, int pageSize = 100)
        //{
        //    try
        //    {
        //        var addressDocs = _context.Get(addressCollection, pageNo, pageSize);              
        //        var addressAggregate = new Collection<AddressAggregate>();
        //        foreach (var item in addressDocs.Records)
        //        {
        //            var address = AddressAggregate.FromJson(item.ToString());
        //            addressAggregate.Add(address);
        //        }

        //        AddressAggregateDetails result = new AddressAggregateDetails()
        //        {
        //            Records = addressAggregate.OrderBy(x => x.AddressId)
        //        };
        //        if (pageNo > 0 || pageSize > 0)
        //        {
        //            result.PagingResult = new PagingResult
        //            {

        //                PageNumber = addressDocs.PageNumber,
        //                PageSize = addressDocs.PageSize,
        //                TotalCount = addressDocs.TotalCount,
        //                TotalPages = addressDocs.TotalPages
        //            };
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //     //   throw new ($"Error occured while fetching", ex);
        //    }

        //}


        //public async Task<AddressAggregate> GetById(string id, string partitionKey)
        //{
        //    try
        //    {
        //        var address = await _context.Get(partitionKey, addressCollection, id);

        //        if (address != null)
        //        {
        //            var resourceAggregate = AddressAggregate.FromJson(address.ToString());

        //            return resourceAggregate;
        //        }
        //        else
        //            return null;

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //        //  throw new ($"Error occured while fetching", ex);
        //    }

        //}
        public async Task<AddressAggregateDetails> Get(int pageNo = 1, int pageSize = 100)
        {
            try
            {
                var addressDocs = await _dbContext.Address.Join(
        _dbContext.Languages,
        address => address.LanguageId,
        language => language.LanguageId,
        (address, language) => new
        {
            id = address.id,
            AddressId = address.AddressId,

            PostCode = address.PostCode,
            PostCodeEdited = address.PostCodeEdited,
            AddressContentId = address.AddressContentId,
            LanguageId = address.LanguageId,
            Street = address.Street,
            City = address.City,
            State = address.State,
            Country = address.Country,
            StreetEdited = address.StreetEdited,
            CityEdited = address.CityEdited,
            StateEdited = address.StateEdited,
            CountryEdited = address.CountryEdited,
            languageDetails = language
        }
    ).Skip((pageNo - 1) * 100).Take(pageSize).ToListAsync();
                var addressAggregate = new Collection<object>();
                foreach (var item in addressDocs)
                {
                    //var address = AddressAggregate.FromJson(item.ToString());
                    addressAggregate.Add(item);
                }

                AddressAggregateDetails result = new AddressAggregateDetails()
                {
                    Records = addressAggregate
                };
                if (pageNo > 0 || pageSize > 0)
                {
                    result.PagingResult = new PagingResult
                    {

                        PageNumber = pageNo,
                        PageSize = pageSize,
                        TotalCount = addressDocs.Count(),
                        TotalPages = (addressDocs.Count() / pageSize)
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
        public async Task<object> GetById(string id, string partitionKey)
        {
            try
            {
                var addressDetails = await _dbContext.Address.Join(
     _dbContext.Languages,
     address => address.LanguageId,
     language => language.LanguageId,
     (address, language) => new
     {
         id = address.id,
         AddressId = address.AddressId,

         PostCode = address.PostCode,
         PostCodeEdited = address.PostCodeEdited,
         AddressContentId = address.AddressContentId,
         LanguageId = address.LanguageId,
         Street = address.Street,
         City = address.City,
         State = address.State,
         Country = address.Country,
         StreetEdited = address.StreetEdited,
         CityEdited = address.CityEdited,
         StateEdited = address.StateEdited,
         CountryEdited = address.CountryEdited,
         languageDetails = language
     }
 ).Where(s => s.id == id).FirstOrDefaultAsync();
                if (addressDetails != null)
                {
                    // var resourceAggregate = AddressAggregate.FromJson(address.ToString());
                    return addressDetails;
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
