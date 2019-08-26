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

        public CreateCountryCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateCountryCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
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
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int pngImageId = _imagesPNG.ImageId > 0 ? _imagesPNG.ImageId : throw new NullReferenceException(nameof(request));
                int svgImageId = _imagesSVG.ImageId > 0 ? _imagesSVG.ImageId : throw new NullReferenceException(nameof(request));
                //  List<Languages> _languages = _CountryRepository.GetAllLanguages();
                Countries _Country = new Countries();
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
                response.IsSuccessful = true;
                scope.Complete();
            }
            var eventSourcing = new CreateArticleCommandEvent()
            {
                EventType = "taxathand",
                Id = "SampleID",
                ArticleName = "SampleName",
                ArticleCountry = "SampleCountry"
            };
            await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
            return response;
        }
    }
}
