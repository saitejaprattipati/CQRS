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

        public UpdateCountryCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<UpdateCountryCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
            _context = new CosmosDBContext();
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
            var countryDocs = _context.GetAll(Constants.CountriesDiscriminator).Records as IEnumerable<CountryCommandEvent>;
            var imageDocs = _context.GetAll(Constants.ImagesDiscriminator).Records as IEnumerable<ImageCommandEvent>;

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
                        country.CountryContents.Remove(resourceContent);
                        _CountryRepository.Delete(resourceContent);
                    }
                }
                country.UpdatedBy = "";
                country.UpdatedDate = DateTime.Now;
                await _CountryRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var pngImageEvent = new ImageCommandEvent()
                {
                    id = imageDocs.ToList().Find(d => d.ImageId == pngImage.ImageId).id,
                    EventType = ServiceBusEventType.Update,//(int)ServiceBusEventType.Update,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = pngImage.ImageId,
                    ImageType = pngImage.ImageType,
                    Name = pngImage.Name,
                    CountryId = pngImage.CountryId,
                    Keyword = pngImage.Keyword,
                    Source = pngImage.Source,
                    Description = pngImage.Description,
                    Copyright = pngImage.Copyright,
                    FilePath = pngImage.FilePath,
                    FileType = pngImage.FileType,
                    CreatedBy = pngImage.CreatedBy,
                    CreatedDate = pngImage.CreatedDate,
                    UpdatedBy = pngImage.UpdatedBy,
                    UpdatedDate = pngImage.UpdatedDate,
                    EmpGuid = pngImage.EmpGuid,
                    IsEdited = true
                };
                await _Eventcontext.PublishThroughEventBusAsync(pngImageEvent);

                var svgImageEvent = new ImageCommandEvent()
                {
                    id = imageDocs.ToList().Find(d => d.ImageId == svgImage.ImageId).id,
                    EventType = ServiceBusEventType.Update,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = svgImage.ImageId,
                    ImageType = svgImage.ImageType,
                    Name = svgImage.Name,
                    CountryId = svgImage.CountryId,
                    Keyword = svgImage.Keyword,
                    Source = svgImage.Source,
                    Description = svgImage.Description,
                    Copyright = svgImage.Copyright,
                    FilePath = svgImage.FilePath,
                    FileType = svgImage.FileType,
                    CreatedBy = svgImage.CreatedBy,
                    CreatedDate = svgImage.CreatedDate,
                    UpdatedBy = svgImage.UpdatedBy,
                    UpdatedDate = svgImage.UpdatedDate,
                    EmpGuid = svgImage.EmpGuid,
                    IsEdited = true
                };
                await _Eventcontext.PublishThroughEventBusAsync(svgImageEvent);

                foreach (var content in country.CountryContents)
                {
                    var eventSourcing = new CountryCommandEvent()
                    {
                        id = countryDocs.ToList().Find(d => d.CountryId == country.CountryId && d.LanguageId == content.LanguageId).id,
                        EventType = ServiceBusEventType.Update,
                        CountryId = country.CountryId,
                        SVGImageId = country.SvgimageId,
                        PNGImageId = country.PngimageId,
                        IsPublished = country.IsPublished,
                        CreatedBy = country.CreatedBy,
                        CreatedDate = country.CreatedDate,
                        UpdatedBy = country.UpdatedBy,
                        UpdatedDate = country.UpdatedDate,
                        CountryContentId = content.CountryContentId,
                        DisplayName = content.DisplayName,
                        DsiplayNameShort = content.DisplayNameShort,
                        LanguageId = content.LanguageId,
                        Discriminator = Constants.CountriesDiscriminator
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
            }
            return response;
        }
    }
}
