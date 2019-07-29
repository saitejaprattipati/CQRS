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
   public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, UpdateCountryCommandResponse>
    {
        private readonly IIntegrationEventBlobService _Eventblobcontext;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryRepository _CountryRepository;
        private readonly ILogger _logger;

        public UpdateCountryCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<UpdateCountryCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext)
        {
            _CountryRepository = new CountryRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
        }
        public async Task<UpdateCountryCommandResponse> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            UpdateCountryCommandResponse response = new UpdateCountryCommandResponse()
            {
                IsSuccessful = false
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {


                List<int> objCountryId = new List<int>();
            objCountryId.Add(request.CountryId);
            var country = _CountryRepository.getCountry(objCountryId)[0];
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
                foreach(Images img in images)
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
            return response;
        }
    }
}
