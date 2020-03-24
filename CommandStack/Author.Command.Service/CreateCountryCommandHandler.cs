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

namespace Author.Command.Service
{
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, CreateCountryCommandResponse>
    {
        private readonly IIntegrationEventBlobService _Eventblobcontext;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryRepository _CountryRepository;
        private readonly ILogger _logger;
        private readonly ICacheService<Countries, Countries> _cacheService;

        public CreateCountryCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateCountryCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext, ICacheService<Countries, Countries> cacheService)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }
        public async Task<CreateCountryCommandResponse> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            CreateCountryCommandResponse response = new CreateCountryCommandResponse()
            {
                IsSuccessful = false
            };
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
            Images _imagesPNG = new Images()
            {
                Name = request.ImagesData.JPGName,
                ImageType = (int)ImageType.FlagPNG,
                FilePath = urls[0],
                FileType = "png",
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "",
                UpdatedDate = DateTime.UtcNow
            };
            Images _imagesSVG = new Images()
            {
                Name = request.ImagesData.SVGName,
                ImageType = (int)ImageType.FlagSVG,
                FilePath = urls[1],
                FileType = "svg",
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "",
                UpdatedDate = DateTime.UtcNow
            };
            _CountryRepository.AddImage(_imagesPNG);
            _CountryRepository.AddImage(_imagesSVG);
            await _CountryRepository.UnitOfWork
                .SaveEntitiesAsync();
            Countries _Country = new Countries();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int pngImageId = _imagesPNG.ImageId > 0 ? _imagesPNG.ImageId : throw new NullReferenceException(nameof(request));
                int svgImageId = _imagesSVG.ImageId > 0 ? _imagesSVG.ImageId : throw new NullReferenceException(nameof(request));
                //  List<Languages> _languages = _CountryRepository.GetAllLanguages();

                _Country.PngimageId = pngImageId;
                _Country.SvgimageId = svgImageId;
                _Country.IsPublished = false;
                _Country.CreatedBy = "";
                _Country.CreatedDate = DateTime.Now;
                _Country.UpdatedBy = "";
                _Country.UpdatedDate = DateTime.Now;
                foreach (var langName in request.LanguageNames)
                {
                    var CountryContent = new CountryContents();
                    CountryContent.DisplayName = langName.CountryName.Trim();
                    CountryContent.DisplayNameShort = langName.ShortName.Trim();
                    CountryContent.LanguageId = langName.LanguageID;
                    _Country.CountryContents.Add(CountryContent);
                }
                _CountryRepository.Add(_Country);
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
                    EventType = ServiceBusEventType.Create,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = _imagesPNG.ImageId,
                    ImageType = _imagesPNG.ImageType,
                    Name = _imagesPNG.Name,
                    CountryId = _imagesPNG.CountryId,
                    Keyword = _imagesPNG.Keyword ?? string.Empty,
                    Source = _imagesPNG.Source ?? string.Empty,
                    Description = _imagesPNG.Description ?? string.Empty,
                    Copyright = _imagesPNG.Copyright ?? string.Empty,
                    FilePath = _imagesPNG.FilePath,
                    FileType = _imagesPNG.FileType,
                    CreatedBy = _imagesPNG.CreatedBy,
                    CreatedDate = _imagesPNG.CreatedDate,
                    UpdatedBy = _imagesPNG.UpdatedBy,
                    UpdatedDate = _imagesPNG.UpdatedDate,
                    EmpGuid = _imagesPNG.EmpGuid ?? string.Empty,
                    IsEdited = false,
                    PartitionKey = ""
                };
                await _Eventcontext.PublishThroughEventBusAsync(pngImageEvent);

                var svgImageEvent = new ImageCommandEvent()
                {
                    EventType = ServiceBusEventType.Create,
                    Discriminator = Constants.ImagesDiscriminator,
                    ImageId = _imagesSVG.ImageId,
                    ImageType = _imagesSVG.ImageType,
                    Name = _imagesSVG.Name,
                    CountryId = _imagesSVG.CountryId,
                    Keyword = _imagesSVG.Keyword ?? string.Empty,
                    Source = _imagesSVG.Source ?? string.Empty,
                    Description = _imagesSVG.Description ?? string.Empty,
                    Copyright = _imagesSVG.Copyright ?? string.Empty,
                    FilePath = _imagesSVG.FilePath,
                    FileType = _imagesSVG.FileType,
                    CreatedBy = _imagesSVG.CreatedBy,
                    CreatedDate = _imagesSVG.CreatedDate,
                    UpdatedBy = _imagesSVG.UpdatedBy,
                    UpdatedDate = _imagesSVG.UpdatedDate,
                    EmpGuid = _imagesSVG.EmpGuid ?? string.Empty,
                    IsEdited = false,
                    PartitionKey = ""
                };
                await _Eventcontext.PublishThroughEventBusAsync(svgImageEvent);

                foreach (var content in _Country.CountryContents)
                {
                    var eventSourcing = new CountryCommandEvent()
                    {
                        EventType = ServiceBusEventType.Create,
                        CountryId = _Country.CountryId,
                        SVGImageId = _Country.SvgimageId,
                        PNGImageId = _Country.PngimageId,
                        IsPublished = _Country.IsPublished,
                        CreatedBy = _Country.CreatedBy,
                        CreatedDate = _Country.CreatedDate,
                        UpdatedBy = _Country.UpdatedBy,
                        UpdatedDate = _Country.UpdatedDate,
                        CountryContentId = content.CountryContentId,
                        DisplayName = content.DisplayName,
                        DisplayNameShort = content.DisplayNameShort,
                        LanguageId = content.LanguageId,
                        Discriminator = Constants.CountriesDiscriminator,
                        PartitionKey = ""
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
