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
using System.Transactions;
using Author.Core.Framework;

namespace Author.Command.Service
{
    public class CreateTagGroupsCommandHandler : IRequestHandler<CreateTagsCommand, TagsCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly TagGroupsRepository _taxTagsRepository;
        private readonly ILogger _logger;

        public CreateTagGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateTagGroupsCommandHandler> logger)
        {
            _taxTagsRepository = new TagGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }
        public async Task<TagsCommandResponse> Handle(CreateTagsCommand request, CancellationToken cancellationToken)
        {
            TagsCommandResponse response = new TagsCommandResponse()
            {
                IsSuccessful = false
            };

            TaxTags _taxTag = new TaxTags();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                _taxTag.IsPublished = true;
                _taxTag.CreatedBy = "";
                _taxTag.CreatedDate = DateTime.Now;
                _taxTag.UpdatedBy = "";
                _taxTag.UpdatedDate = DateTime.Now;
                _taxTag.ParentTagId = null;
                if (request.TagType == "Tag")
                {
                    _taxTag.ParentTagId = request.TagGroup;
                    foreach (var country in request.RelatedCountyIds)
                    {
                        TaxTagRelatedCountries objRelatedCountries = new TaxTagRelatedCountries();
                        objRelatedCountries.CountryId = country;
                        _taxTag.TaxTagRelatedCountries.Add(objRelatedCountries);
                    }
                }
                foreach (var langName in request.LanguageName)
                {
                    var taxTagsContent = new TaxTagContents();
                    taxTagsContent.DisplayName = langName.Name.Trim();
                    taxTagsContent.LanguageId = langName.LanguageId;
                    _taxTag.TaxTagContents.Add(taxTagsContent);
                }
                _taxTagsRepository.Add(_taxTag);
                await _taxTagsRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var content in _taxTag.TaxTagContents)
                {
                    var eventSourcing = new TagGroupCommandEvent()
                    {
                        EventType = ServiceBusEventType.Create,
                        Discriminator = Constants.TaxTagsDiscriminator,
                        TagId = _taxTag.TaxTagId,
                        ParentTagId = _taxTag.ParentTagId,
                        IsPublished = _taxTag.IsPublished,
                        CreatedBy = _taxTag.CreatedBy,
                        CreatedDate = _taxTag.CreatedDate,
                        UpdatedBy = _taxTag.UpdatedBy,
                        UpdatedDate = _taxTag.UpdatedDate,
                        RelatedCountryIds = (from rc in _taxTag.TaxTagRelatedCountries where rc != null select rc.CountryId).ToList(),
                        TagContentId = content.TaxTagContentId,
                        LanguageId = content.LanguageId,
                        DisplayName = content.DisplayName,
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