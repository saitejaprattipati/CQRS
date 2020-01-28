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
using Author.Core.Framework.ExceptionHandling;
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;

namespace Author.Command.Service
{
   public class UpdateTagGroupsCommandHandler : IRequestHandler<UpdateTagsCommand, UpdateTagGroupsCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly TagGroupsRepository _taxTagsRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;


        public UpdateTagGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateTagGroupsCommandHandler> logger)
        {
            _taxTagsRepository = new TagGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }
        public async Task<UpdateTagGroupsCommandResponse> Handle(UpdateTagsCommand request, CancellationToken cancellationToken)
        {
            UpdateTagGroupsCommandResponse response = new UpdateTagGroupsCommandResponse()
            {
                IsSuccessful = false
            };
            List<int> objTagGroups = new List<int>();
            objTagGroups.Add(request.TagGroupsId);
            var taxGroup = _taxTagsRepository.GetTagGroups(objTagGroups)[0];
            var contentToDelete = new List<int>();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //List<Languages> languages = _taxTagsRepository.GetAllLanguages();
                if (request.TagType == "Tag")
                {
                    if(taxGroup.ParentTagId==null) throw new RulesException("Invalid", @"Tag not Valid");
                    taxGroup.ParentTagId = request.TagGroup;
                    foreach (var country in request.RelatedCountyIds)
                    {
                        var taxCountries = taxGroup.TaxTagRelatedCountries.Where(s => s.CountryId == country).FirstOrDefault();
                        if(taxCountries == null)
                        { 
                        TaxTagRelatedCountries objRelatedCountries = new TaxTagRelatedCountries();
                        objRelatedCountries.CountryId = country;
                        taxGroup.TaxTagRelatedCountries.Add(objRelatedCountries);
                        }
                        else
                        {
                            taxCountries.CountryId = country;
                            _taxTagsRepository.Update(taxCountries);
                        }
                    }
                }
                foreach (var content in request.LanguageName)
                {
                    var taxGroupContents = taxGroup.TaxTagContents.Where(s => s.LanguageId == content.LanguageId).FirstOrDefault();
                    if (taxGroupContents == null)
                    {
                        TaxTagContents objtaxGroupContents = new TaxTagContents();
                        objtaxGroupContents.DisplayName = content.Name;
                        objtaxGroupContents.LanguageId = content.LanguageId;
                        taxGroup.TaxTagContents.Add(objtaxGroupContents);
                    }
                    else
                    {
                        taxGroupContents.DisplayName = content.Name;
                        _taxTagsRepository.Update(taxGroupContents);
                    }
                }
                //  List<TaxTagContents> ResourceGroupContents = taxGroup.TaxTagContents.Where(s => s.TaxTagId == request.TagGroupsId).ToList();
                foreach (var resourceContent in taxGroup.TaxTagContents.ToList())
                {
                    if (request.LanguageName.Where(s => s.LanguageId == resourceContent.LanguageId).Count() == 0)
                    {
                        contentToDelete.Add((int)resourceContent.LanguageId);
                        taxGroup.TaxTagContents.Remove(resourceContent);
                        _taxTagsRepository.Delete(resourceContent);
                    }
                }
                foreach (var resourceCountries in taxGroup.TaxTagRelatedCountries.ToList())
                {
                    if (request.RelatedCountyIds.Where(s =>s == resourceCountries.CountryId).Count() == 0)
                    {
                        taxGroup.TaxTagRelatedCountries.Remove(resourceCountries);
                        _taxTagsRepository.Delete(resourceCountries);
                    }
                }
                taxGroup.UpdatedBy = "";
                taxGroup.UpdatedDate = DateTime.Now;
                await _taxTagsRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var taggroupDocs = _context.GetAll(Constants.TaxTagsDiscriminator);
                foreach (var content in taxGroup.TaxTagContents)
                {
                    var doc = taggroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("TaxTagId") == taxGroup.TaxTagId
                                && d.GetPropertyValue<int?>("LanguageId") == content.LanguageId);
                    var eventSourcing = new TagGroupCommandEvent()
                    {
                        id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                        EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                        Discriminator = Constants.TaxTagsDiscriminator,
                        TagId = taxGroup.TaxTagId,
                        ParentTagId = taxGroup.ParentTagId,
                        IsPublished = taxGroup.IsPublished,
                        CreatedBy = taxGroup.CreatedBy,
                        CreatedDate = taxGroup.CreatedDate,
                        UpdatedBy = taxGroup.UpdatedBy,
                        UpdatedDate = taxGroup.UpdatedDate,
                        RelatedCountryIds = (from rc in taxGroup.TaxTagRelatedCountries where rc != null select rc.CountryId).ToList(),
                        TagContentId = content.TaxTagContentId,
                        LanguageId = content.LanguageId,
                        DisplayName = content.DisplayName
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach(int i in contentToDelete)
                {
                    var deleteEvt = new TagGroupCommandEvent()
                    {
                        id = taggroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("TagId") == taxGroup.TaxTagId
                              && d.GetPropertyValue<int?>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.TaxTagsDiscriminator
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(deleteEvt);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
