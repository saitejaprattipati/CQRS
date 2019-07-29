//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Author.Core.Framework.ExceptionHandling;
using System.Transactions;
namespace Author.Command.Service
{
   public class ManipulateCountriesCommandHandler : IRequestHandler<ManipulateCountriesCommand, ManipulateCountriesCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryRepository _CountryRepository;
        private readonly ILogger _logger;
        public ManipulateCountriesCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateCountriesCommandHandler> logger)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }

        public async Task<ManipulateCountriesCommandResponse> Handle(ManipulateCountriesCommand request, CancellationToken cancellationToken)
        {
            ManipulateCountriesCommandResponse response = new ManipulateCountriesCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<Countries> countries = _CountryRepository.getCountry(request.ResourceGroupIds);
                if (request.ResourceGroupIds.Count != countries.Count)
                    throw new RulesException("Invalid", @"ResourceGroup not found");

                if (request.Operation == "Publish")
                {
                    foreach (var country in countries)
                    {
                        country.IsPublished = true;
                        _CountryRepository.Update<Countries>(country);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var country in countries)
                    {
                        country.IsPublished = false;
                        _CountryRepository.Update<Countries>(country);
                    }
                }
                else if (request.Operation == "Delete")
                {
                    
                    foreach (Countries country in countries)
                    {
                        List<Images> images = _CountryRepository.getImages(new List<int?> { country.PngimageId, country.SvgimageId });
                        foreach (var image in images)
                        {
                            _CountryRepository.DeleteImage(image);
                        }

                        foreach (var countryContents in country.CountryContents.ToList())
                        {
                            country.CountryContents.Remove(countryContents);
                            _CountryRepository.Delete<CountryContents>(countryContents);
                        }
                        _CountryRepository.DeleteCountry(country);
                    }
                    
                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _CountryRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
