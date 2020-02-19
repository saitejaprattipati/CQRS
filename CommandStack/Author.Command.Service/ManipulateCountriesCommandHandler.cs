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

            var countryDocs = _context.GetAll(Constants.CountriesDiscriminator);
            var imageDocs = _context.GetAll(Constants.ImagesDiscriminator);
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish" || request.Operation == "UnPublish")
                {
                    foreach (var country in countries)
                    {
                        foreach (var doc in countryDocs.Where(d => d.GetPropertyValue<int>("CountryId") == country.CountryId))
                        {
                            var eventsource = new CountryCommandEvent()
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Update,
                                Discriminator = Constants.CountriesDiscriminator,
                                CountryId = country.CountryId,
                                IsPublished = country.IsPublished,
                                CreatedBy = doc.GetPropertyValue<string>("CreatedBy"),
                                CreatedDate = doc.GetPropertyValue<DateTime>("CreatedDate"),
                                UpdatedBy = doc.GetPropertyValue<string>("UpdatedBy"),
                                UpdatedDate = doc.GetPropertyValue<DateTime>("UpdatedDate"),
                                PNGImageId = doc.GetPropertyValue<int?>("PNGImageId"),
                                SVGImageId = doc.GetPropertyValue<int?>("SVGImageId"),
                                CountryContentId = doc.GetPropertyValue<int>("CountryContentId"),
                                DisplayName = doc.GetPropertyValue<string>("DisplayName"),
                                DisplayNameShort = doc.GetPropertyValue<string>("DisplayNameShort"),
                                LanguageId = doc.GetPropertyValue<int?>("LanguageId"),
                                PartitionKey = ""
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventsource);
                        }
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (var item in countries)
                    {                        
                        var pngimgevent = new ImageCommandEvent()
                        {
                            id = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == item.PngimageId).GetPropertyValue<Guid>("id"),
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.ImagesDiscriminator,
                            PartitionKey = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == item.SvgimageId).GetPropertyValue<int>("CountryId").ToString()
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(pngimgevent);
                        var svgimgevent = new ImageCommandEvent()
                        {
                            id = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == item.SvgimageId).GetPropertyValue<Guid>("id"),
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.ImagesDiscriminator,
                            PartitionKey = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == item.SvgimageId).GetPropertyValue<int>("CountryId").ToString()
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(svgimgevent);

                        foreach (var doc in countryDocs.Where(d => d.GetPropertyValue<int>("CountryId") == item.CountryId)) 
                        {
                            var countryevent = new CountryCommandEvent()
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Delete,
                                Discriminator = Constants.CountriesDiscriminator,
                                PartitionKey = doc.GetPropertyValue<int>("LanguageId").ToString()
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(countryevent);
                        }
                    }
                }
                scope.Complete();
            }
            return response;
        }
    }
}
