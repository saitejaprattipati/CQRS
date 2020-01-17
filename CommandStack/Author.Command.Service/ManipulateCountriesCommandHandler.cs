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
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;

namespace Author.Command.Service
{
    public class ManipulateCountriesCommandHandler : IRequestHandler<ManipulateCountriesCommand, ManipulateCountriesCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryRepository _CountryRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;

        public ManipulateCountriesCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateCountriesCommandHandler> logger)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }

        public async Task<ManipulateCountriesCommandResponse> Handle(ManipulateCountriesCommand request, CancellationToken cancellationToken)
        {
            ManipulateCountriesCommandResponse response = new ManipulateCountriesCommandResponse()
            {
                IsSuccessful = false
            };

            List<Countries> countries = _CountryRepository.getCountry(request.CountryIds);
            if (request.CountryIds.Count != countries.Count)
                throw new RulesException("Invalid", @"Country not found");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
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

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish" || request.Operation == "UnPublish")
                {
                    var countryDocs = _context.GetAll(Constants.CountriesDiscriminator).Records as IEnumerable<CountryCommandEvent>;
                    foreach (var country in countries)
                    {
                        foreach (var doc in countryDocs.Where(d => d.CountryId == country.CountryId))
                        {
                            var eventsource = new CountryCommandEvent()
                            {
                                id = doc.id,
                                EventType = ServiceBusEventType.Update,
                                Discriminator = Constants.CountriesDiscriminator,
                                CountryId = country.CountryId,
                                IsPublished = country.IsPublished
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventsource);
                        }
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (var country in countries)
                    {
                        foreach (var img in country.Images)
                        {
                            var imgevent = new ImageCommandEvent()
                            {
                                EventType = ServiceBusEventType.Delete,
                                Discriminator = Constants.ImagesDiscriminator,
                                ImageId = img.ImageId
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(imgevent);
                        }
                        var countryevent = new CountryCommandEvent()
                        {
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.CountriesDiscriminator,
                            CountryId = country.CountryId
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(countryevent);
                    }
                }
            }
            return response;
        }
    }
}
