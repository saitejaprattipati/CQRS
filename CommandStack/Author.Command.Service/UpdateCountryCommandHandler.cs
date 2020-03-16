//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
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
using System.Transactions;
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;

namespace Author.Command.Service
{
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, UpdateCountryCommandResponse>
    {
        private readonly IIntegrationEventBlobService _Eventblobcontext;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryRepository _CountryRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;
        private readonly ICacheService<Countries, Countries> _cacheService;

        public UpdateCountryCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<UpdateCountryCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext, ICacheService<Countries, Countries> cacheService)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
            _context = new CosmosDBContext();
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }
        public async Task<UpdateCountryCommandResponse> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            UpdateCountryCommandResponse response = new UpdateCountryCommandResponse()
            {
                IsSuccessful = false
            };

            var country = new Countries();
            Images pngImage = new Images();
            Images svgImage = new Images();
            var countryDocs = _context.GetAll(Constants.CountriesDiscriminator);
            var imageDocs = _context.GetAll(Constants.ImagesDiscriminator);
            var contentToDelete = new List<int>();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<int> objCountryId = new List<int>();
                objCountryId.Add(request.CountryId);
                country = _CountryRepository.getCountry(objCountryId)[0];
                List<Images> images = _CountryRepository.getImages(new List<int?> { country.PngimageId, country.SvgimageId });

                if (request.ImagesData.JPGData == "" || request.ImagesData.JPGName == "" || request.ImagesData.SVGData == "" || request.ImagesData.SVGData == "")
                { throw new ArgumentNullException(nameof(request)); }
                ImageData imageData = new ImageData()
                {
                    JPGData = request.ImagesData.JPGData,
                    JPGName = request.ImagesData.JPGName,
                    SVGData = request.ImagesData.SVGData,
                    SVGName = request.ImagesData.SVGName
                };
                List<string> urls = await _Eventblobcontext.PublishThroughBlobStorageAsync(imageData);
                foreach (Images img in images)
                {
                    if (img.FileType == "svg")
                    {
                        img.Name = request.ImagesData.SVGName;
                        img.ImageType = (int)ImageType.FlagSVG;
                        img.FilePath = urls[1];
                        img.FileType = "svg";
                        img.UpdatedBy = "";
                        img.UpdatedDate = DateTime.UtcNow;
                        _CountryRepository.Update(img);
                        svgImage = img;
                    }
                    else
                    {
                        img.Name = request.ImagesData.JPGName;
                        img.ImageType = (int)ImageType.FlagPNG;
                        img.FilePath = urls[0];
                        img.FileType = "png";
                        img.UpdatedBy = "";
                        img.UpdatedDate = DateTime.UtcNow;
                        _CountryRepository.Update(img);
                        pngImage = img;
                    }
                }
                foreach (var content in request.LanguageNames)
                {
                    var countryContents = country.CountryContents.Where(s => s.LanguageId == content.LanguageID).FirstOrDefault();
                    if (countryContents == null)
                    {
                        CountryContents objCountryContents = new CountryContents();
                        objCountryContents.DisplayName = content.CountryName;
                        objCountryContents.DisplayNameShort = content.ShortName;
                        objCountryContents.LanguageId = content.LanguageID;
                        country.CountryContents.Add(objCountryContents);
                    }
                    else
                    {
                        countryContents.DisplayName = content.CountryName;
                        countryContents.DisplayNameShort = content.ShortName;
                        countryContents.LanguageId = content.LanguageID;
                        _CountryRepository.Update(countryContents);
                    }
                }
                //    List<ResourceGroupContents> ResourceGroupContents = resourceGroup.ResourceGroupContents.Where(s => s.ResourceGroupId == request.ResourceGroupId).ToList();
                foreach (var resourceContent in country.CountryContents.ToList())
                {
                    if (request.LanguageNames.Where(s => s.LanguageID == resourceContent.LanguageId).Count() == 0)
                    {
                        contentToDelete.Add((int)resourceContent.LanguageId);
                        country.CountryContents.Remove(resourceContent);
                        _CountryRepository.Delete(resourceContent);
                    }
                }
                country.UpdatedBy = "";
                country.UpdatedDate = DateTime.Now;
                await _CountryRepository.UnitOfWork
                   .SaveEntitiesAsync();
                await _cacheService.ClearCacheAsync("countriesCacheKey");
                await _cacheService.ClearCacheAsync("imagesCacheKey");
                response.IsSuccessful = true;
                scope.Complete();
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var pngImageEvent = new ImageCommandEvent()
                {
                    id = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == pngImage.ImageId).GetPropertyValue<Guid>("id"),
                    EventType = ServiceBusEventType.Update,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = pngImage.ImageId,
                    ImageType = pngImage.ImageType,
                    Name = pngImage.Name,
                    CountryId = pngImage.CountryId,
                    Keyword = pngImage.Keyword ?? string.Empty,
                    Source = pngImage.Source ?? string.Empty,
                    Description = pngImage.Description ?? string.Empty,
                    Copyright = pngImage.Copyright ?? string.Empty,
                    FilePath = pngImage.FilePath,
                    FileType = pngImage.FileType,
                    CreatedBy = pngImage.CreatedBy,
                    CreatedDate = pngImage.CreatedDate,
                    UpdatedBy = pngImage.UpdatedBy,
                    UpdatedDate = pngImage.UpdatedDate,
                    EmpGuid = pngImage.EmpGuid ?? string.Empty,
                    IsEdited = true,
                    PartitionKey = ""
                };
                await _Eventcontext.PublishThroughEventBusAsync(pngImageEvent);

                var svgImageEvent = new ImageCommandEvent()
                {
                    id = imageDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ImageId") == svgImage.ImageId).GetPropertyValue<Guid>("id"),
                    EventType = ServiceBusEventType.Update,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = svgImage.ImageId,
                    ImageType = svgImage.ImageType,
                    Name = svgImage.Name,
                    CountryId = svgImage.CountryId,
                    Keyword = svgImage.Keyword ?? string.Empty,
                    Source = svgImage.Source ?? string.Empty,
                    Description = svgImage.Description ??string.Empty,
                    Copyright = svgImage.Copyright ?? string.Empty,
                    FilePath = svgImage.FilePath,
                    FileType = svgImage.FileType,
                    CreatedBy = svgImage.CreatedBy,
                    CreatedDate = svgImage.CreatedDate,
                    UpdatedBy = svgImage.UpdatedBy,
                    UpdatedDate = svgImage.UpdatedDate,
                    EmpGuid = svgImage.EmpGuid ?? string.Empty,
                    IsEdited = true,
                    PartitionKey = ""
                };
                await _Eventcontext.PublishThroughEventBusAsync(svgImageEvent);

                foreach (var item in country.CountryContents)
                {
                    var doc = countryDocs.FirstOrDefault(d => d.GetPropertyValue<int>("CountryId") == country.CountryId
                             && d.GetPropertyValue<int?>("LanguageId") == item.LanguageId);
                    var eventSourcing = new CountryCommandEvent()
                    {
                        id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                        EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                        CountryId = country.CountryId,
                        SVGImageId = country.SvgimageId,
                        PNGImageId = country.PngimageId,
                        IsPublished = country.IsPublished,
                        CreatedBy = country.CreatedBy,
                        CreatedDate = country.CreatedDate,
                        UpdatedBy = country.UpdatedBy,
                        UpdatedDate = country.UpdatedDate,
                        CountryContentId = item.CountryContentId,
                        DisplayName = item.DisplayName,
                        DisplayNameShort = item.DisplayNameShort,
                        LanguageId = item.LanguageId,
                        Discriminator = Constants.CountriesDiscriminator,
                        PartitionKey = ""
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach(int i in contentToDelete)
                {
                    var deleteEvt = new CountryCommandEvent()
                    {
                        id = countryDocs.FirstOrDefault(d => d.GetPropertyValue<int>("CountryId") == country.CountryId &&
                                 d.GetPropertyValue<int>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.CountriesDiscriminator,
                        PartitionKey = ""
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(deleteEvt);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
