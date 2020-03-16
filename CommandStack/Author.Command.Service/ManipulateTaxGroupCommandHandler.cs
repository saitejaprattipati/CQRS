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
using Author.Core.Services.Persistence.CosmosDB;
using Author.Core.Framework;
using Author.Command.Domain.Models;

namespace Author.Command.Service
{
    public class ManipulateTaxGroupCommandHandler : IRequestHandler<ManipulateTaxGroupCommand, ManipulateTaxGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly TagGroupsRepository _tagGroupRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;

        public ManipulateTaxGroupCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateTaxGroupCommandHandler> logger)
        {
            _tagGroupRepository = new TagGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }

        public async Task<ManipulateTaxGroupCommandResponse> Handle(ManipulateTaxGroupCommand request, CancellationToken cancellationToken)
        {
            ManipulateTaxGroupCommandResponse response = new ManipulateTaxGroupCommandResponse()
            {
                IsSuccessful = false
            };

            List<TaxTags> tagGroups = _tagGroupRepository.GetTagGroups(request.TaxGroupIds);
            if (request.TaxGroupIds.Count != tagGroups.Count)
                throw new RulesException("Invalid", @"TagGroup not found");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish")
                {
                    foreach (var taxgroup in tagGroups)
                    {
                        if (taxgroup.ParentTagId == null && request.TagType == "Tag") throw new RulesException("Invalid", @"Tag not Valid");
                        taxgroup.IsPublished = true;
                        _tagGroupRepository.Update<TaxTags>(taxgroup);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var taxgroup in tagGroups)
                    {
                        if (taxgroup.ParentTagId == null && request.TagType == "Tag") throw new RulesException("Invalid", @"Tag not Valid");
                        taxgroup.IsPublished = false;
                        _tagGroupRepository.Update<TaxTags>(taxgroup);
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (TaxTags taxgroup in tagGroups)
                    {
                        foreach (var tagGroupContents in taxgroup.TaxTagContents.ToList())
                        {
                            taxgroup.TaxTagContents.Remove(tagGroupContents);
                            _tagGroupRepository.Delete<TaxTagContents>(tagGroupContents);
                        }
                        if (request.TagType == "Tag")
                        {
                            if (taxgroup.ParentTagId == null) throw new RulesException("Invalid", @"Tag not Valid");
                            foreach (var country in taxgroup.TaxTagRelatedCountries.ToList())
                            {
                                taxgroup.TaxTagRelatedCountries.Remove(country);
                                _tagGroupRepository.Delete<TaxTagRelatedCountries>(country);
                            }
                        }
                        _tagGroupRepository.DeletetagGroup(taxgroup);
                    }

                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _tagGroupRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var taxtagDocs = _context.GetAll(Constants.TaxTagsDiscriminator);

                if (request.Operation == "Publish" || request.Operation == "UnPublish")
                {
                    foreach (var tagGrp in tagGroups)
                    {
                        foreach (var doc in taxtagDocs.Where(d => d.GetPropertyValue<int>("TaxTagId") == tagGrp.TaxTagId))
                        {
                            var eventSource = new TagGroupCommandEvent
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Update,
                                Discriminator = Constants.TaxTagsDiscriminator,
                                IsPublished = tagGrp.IsPublished,
                                TagId = tagGrp.TaxTagId,
                                UpdatedDate = doc.GetPropertyValue<DateTime>("UpdatedDate"),
                                UpdatedBy = doc.GetPropertyValue<string>("UpdatedBy"),
                                CreatedDate = doc.GetPropertyValue<DateTime>("CreatedDate"),
                                CreatedBy = doc.GetPropertyValue<string>("CreatedBy"),
                                DisplayName = doc.GetPropertyValue<string>("DisplayName"),
                                LanguageId = doc.GetPropertyValue<int?>("LanguageId"),
                                ParentTagId = doc.GetPropertyValue<int?>("ParentTagId"),
                                RelatedCountryIds = doc.GetPropertyValue<List<int>>("RelatedCountryIds"),
                                TagContentId = doc.GetPropertyValue<int>("TaxTagContentId"),
                                PartitionKey = doc.GetPropertyValue<int>("LanguageId").ToString()
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventSource);
                        }
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (var tagGrp in tagGroups)
                    {
                        foreach (var content in tagGrp.TaxTagContents)
                        {
                            var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
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
                                            relatedTaxTagsSchema.Remove(relatedTaxTagsSchema.Where(i => i.TaxTagId == tagGrp.TaxTagId).First());
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
                        }
                        foreach (var doc in taxtagDocs.Where(d => d.GetPropertyValue<int>("TaxTagId") == tagGrp.TaxTagId))
                        {
                            var eventSrc = new TagGroupCommandEvent
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Delete,
                                Discriminator = Constants.TaxTagsDiscriminator,
                                PartitionKey = ""
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventSrc);
                        }
                    }
                }
                scope.Complete();
            }
            return response;
        }
    }
}
