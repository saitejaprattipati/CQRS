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
using Author.Command.Domain.Models;

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
            var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
            var taxTagDocs = _context.GetAll(Constants.TaxTagsDiscriminator);
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //List<Languages> languages = _taxTagsRepository.GetAllLanguages();
                if (request.TagType == "Tag")
                {
                    if (taxGroup.ParentTagId == null) throw new RulesException("Invalid", @"Tag not Valid");
                    taxGroup.ParentTagId = request.TagGroup;
                    foreach (var country in request.RelatedCountyIds)
                    {
                        var taxCountries = taxGroup.TaxTagRelatedCountries.Where(s => s.CountryId == country).FirstOrDefault();
                        if (taxCountries == null)
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
                    if (request.RelatedCountyIds.Where(s => s == resourceCountries.CountryId).Count() == 0)
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
                    foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == content.LanguageId))
                    {
                        foreach (var relatedTaxTags in article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags"))
                        {
                            if (relatedTaxTags.TaxTagId == content.TaxTagId)
                            {
                                List<RelatedTaxTagsSchema> relatedTaxTagsSchema = new List<RelatedTaxTagsSchema>();
                                relatedTaxTagsSchema = article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags");

                                var index = relatedTaxTagsSchema.IndexOf(relatedTaxTagsSchema.Where(i => i.TaxTagId == content.TaxTagId).First());
                                if (index != -1)
                                    relatedTaxTagsSchema[index] = new RelatedTaxTagsSchema { TaxTagId = int.Parse(content.TaxTagId.ToString()), DisplayName = content.DisplayName };
                                var eventSourcingRelated = new ArticleCommandEvent()
                                {
                                    id = article != null ? article.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                                    EventType = ServiceBusEventType.Update,
                                    ArticleId = article.GetPropertyValue<int>("ArticleId"),
                                    PublishedDate = article.GetPropertyValue<string>("PublishedDate"),
                                    Author = article.GetPropertyValue<string>("author"),
                                    ImageId = article.GetPropertyValue<int>("ImageId"),
                                    State = article.GetPropertyValue<string>("State"),
                                    Type = article.GetPropertyValue<int>("Type"),
                                    SubType = article.GetPropertyValue<int>("SubType"),
                                    ResourcePosition = article.GetPropertyValue<int>("ResourcePosition"),
                                    Disclaimer = article.GetPropertyValue<DisclamersSchema>("Disclaimer"),
                                    ResourceGroup = article.GetPropertyValue<ResourceGroupsSchema>("ResourceGroup"),
                                    IsPublished = article.GetPropertyValue<bool>("IsPublished"),
                                    CreatedDate = article.GetPropertyValue<string>("CreatedDate"),
                                    CreatedBy = article.GetPropertyValue<string>("CreatedBy"),
                                    UpdatedDate = article.GetPropertyValue<string>("UpdatedDate"),
                                    UpdatedBy = article.GetPropertyValue<string>("UpdatedBy"),
                                    NotificationSentDate = article.GetPropertyValue<string>("NotificationSentDate"),
                                    Provinces = article.GetPropertyValue<ProvinceSchema>("Provisions"),
                                    ArticleContentId = article.GetPropertyValue<int>("ArticleContentId"),
                                    LanguageId = article.GetPropertyValue<int>("LanguageId"),
                                    Title = article.GetPropertyValue<string>("Title"),
                                    TitleInEnglishDefault = article.GetPropertyValue<string>("TitleInEnglishDefault"),
                                    TeaserText = article.GetPropertyValue<string>("TeaserText"),
                                    Content = article.GetPropertyValue<string>("Content"),
                                    RelatedContacts = article.GetPropertyValue<List<RelatedEntityId>>("RelatedContacts"),
                                    RelatedCountries = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountries"),
                                    RelatedCountryGroups = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountryGroups"),
                                    RelatedTaxTags = relatedTaxTagsSchema,
                                    RelatedArticles = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles"),
                                    RelatedResources = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedResources"),
                                    Discriminator = article.GetPropertyValue<string>("Discriminator"),
                                    PartitionKey = ""
                                };
                                await _Eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                            }
                        }
                    }
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
                        DisplayName = content.DisplayName,
                        PartitionKey = ""
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach (int i in contentToDelete)
                {
                    foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == i))
                    {
                        foreach (var RelatedTaxTags in article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags"))
                        {
                            if (RelatedTaxTags.TaxTagId == taxGroup.TaxTagId)
                            {
                                var DisplayNameInEnglish = taxTagDocs.Where(ad => ad.GetPropertyValue<int>("TaxTagId") == taxGroup.TaxTagId && ad.GetPropertyValue<int>("LanguageId") == 37).Select(ads => ads.GetPropertyValue<string>("DisplayName")).FirstOrDefault();
                                List<RelatedTaxTagsSchema> relatedTaxTagsSchema = new List<RelatedTaxTagsSchema>();
                                relatedTaxTagsSchema = article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags");

                                var index = relatedTaxTagsSchema.IndexOf(relatedTaxTagsSchema.Where(ras => ras.TaxTagId == taxGroup.TaxTagId).First());
                                if (index != -1)
                                    if (DisplayNameInEnglish == "") relatedTaxTagsSchema.Remove(relatedTaxTagsSchema.Where(rtt => rtt.TaxTagId == taxGroup.TaxTagId).First()); else relatedTaxTagsSchema[index] = new RelatedTaxTagsSchema { TaxTagId = taxGroup.TaxTagId, DisplayName = (DisplayNameInEnglish == null ? "" : DisplayNameInEnglish) };
                                var eventSourcingRelated = new ArticleCommandEvent()
                                {
                                    id = article != null ? article.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                                    EventType = ServiceBusEventType.Update,
                                    ArticleId = article.GetPropertyValue<int>("ArticleId"),
                                    PublishedDate = article.GetPropertyValue<string>("PublishedDate"),
                                    Author = article.GetPropertyValue<string>("author"),
                                    ImageId = article.GetPropertyValue<int>("ImageId"),
                                    State = article.GetPropertyValue<string>("State"),
                                    Type = article.GetPropertyValue<int>("Type"),
                                    SubType = article.GetPropertyValue<int>("SubType"),
                                    ResourcePosition = article.GetPropertyValue<int>("ResourcePosition"),
                                    Disclaimer = article.GetPropertyValue<DisclamersSchema>("Disclaimer"),
                                    ResourceGroup = article.GetPropertyValue<ResourceGroupsSchema>("ResourceGroup"),
                                    IsPublished = article.GetPropertyValue<bool>("IsPublished"),
                                    CreatedDate = article.GetPropertyValue<string>("CreatedDate"),
                                    CreatedBy = article.GetPropertyValue<string>("CreatedBy"),
                                    UpdatedDate = article.GetPropertyValue<string>("UpdatedDate"),
                                    UpdatedBy = article.GetPropertyValue<string>("UpdatedBy"),
                                    NotificationSentDate = article.GetPropertyValue<string>("NotificationSentDate"),
                                    Provinces = article.GetPropertyValue<ProvinceSchema>("Provisions"),
                                    ArticleContentId = article.GetPropertyValue<int>("ArticleContentId"),
                                    LanguageId = article.GetPropertyValue<int>("LanguageId"),
                                    Title = article.GetPropertyValue<string>("Title"),
                                    TitleInEnglishDefault = article.GetPropertyValue<string>("TitleInEnglishDefault"),
                                    TeaserText = article.GetPropertyValue<string>("TeaserText"),
                                    Content = article.GetPropertyValue<string>("Content"),
                                    RelatedContacts = article.GetPropertyValue<List<RelatedEntityId>>("RelatedContacts"),
                                    RelatedCountries = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountries"),
                                    RelatedCountryGroups = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountryGroups"),
                                    RelatedTaxTags = relatedTaxTagsSchema,
                                    RelatedArticles = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles"),
                                    RelatedResources = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedResources"),
                                    Discriminator = article.GetPropertyValue<string>("Discriminator"),
                                    PartitionKey = ""
                                };
                                await _Eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                            }
                        }
                    }
                    var deleteEvt = new TagGroupCommandEvent()
                    {
                        id = taggroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("TagId") == taxGroup.TaxTagId
                              && d.GetPropertyValue<int?>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.TaxTagsDiscriminator,
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
