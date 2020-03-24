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
using Author.Command.Domain.Models;

namespace Author.Command.Service
{
    public class ManipulateResourceGroupCommandHandler : IRequestHandler<ManipulateResourceGroupCommand, ManipulateResourceGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ResourceGroupRepository _ResourceGroupRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;

        public ManipulateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateResourceGroupCommandHandler> logger)
        {
            _ResourceGroupRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }

        public async Task<ManipulateResourceGroupCommandResponse> Handle(ManipulateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            ManipulateResourceGroupCommandResponse response = new ManipulateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };
            var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
            List<ResourceGroups> resourceGroups = _ResourceGroupRepository.getResourceGroups(request.ResourceGroupIds);
            if (request.ResourceGroupIds.Count != resourceGroups.Count)
                throw new RulesException("Invalid", @"ResourceGroup not found");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish")
                {
                    foreach (var resourcegroup in resourceGroups)
                    {
                        resourcegroup.IsPublished = true;
                        _ResourceGroupRepository.Update<ResourceGroups>(resourcegroup);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var resourcegroup in resourceGroups)
                    {
                        resourcegroup.IsPublished = false;
                        _ResourceGroupRepository.Update<ResourceGroups>(resourcegroup);
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (ResourceGroups resourcegroup in resourceGroups)
                    {
                        foreach (var resourceGroupContents in resourcegroup.ResourceGroupContents.ToList())
                        {
                            resourcegroup.ResourceGroupContents.Remove(resourceGroupContents);
                            _ResourceGroupRepository.Delete<ResourceGroupContents>(resourceGroupContents);
                        }
                        _ResourceGroupRepository.DeleteResourceGroup(resourcegroup);
                    }
                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _ResourceGroupRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var resourcegrpDocs = _context.GetAll(Constants.ResourceGroupsDiscriminator);

                if (request.Operation == "Publish" || request.Operation == "UnPublish")
                {
                    foreach (var resourcegrp in resourceGroups)
                    {
                        foreach (var doc in resourcegrpDocs.Where(d => d.GetPropertyValue<int>("ResourceGroupId") == resourcegrp.ResourceGroupId))
                        {
                            var eventsource = new ResourceGroupCommandEvent()
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Update,
                                Discriminator = Constants.ResourceGroupsDiscriminator,
                                ResourceGroupId = resourcegrp.ResourceGroupId,
                                IsPublished = resourcegrp.IsPublished,
                                LanguageId = doc.GetPropertyValue<int?>("LanguageId"),
                                GroupName = doc.GetPropertyValue<string>("GroupName"),
                                Position = doc.GetPropertyValue<int>("Position"),
                                ResourceGroupContentId = doc.GetPropertyValue<int>("ResourceGroupContentId"),
                                CreatedBy = doc.GetPropertyValue<string>("CreatedBy"),
                                CreatedDate = doc.GetPropertyValue<DateTime>("CreatedDate"),
                                UpdatedBy = doc.GetPropertyValue<string>("UpdatedBy"),
                                UpdatedDate = doc.GetPropertyValue<DateTime>("UpdatedDate"),
                                PartitionKey = ""
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventsource);
                        }
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (var resourcegrp in resourceGroups)
                    {
                        foreach (var content in resourcegrp.ResourceGroupContents)
                        {
                            foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == content.LanguageId))
                        {
                            var relatedResourceGroups = article.GetPropertyValue<List<ResourceGroupsSchema>>("ResourceGroup").FirstOrDefault();
                            if (relatedResourceGroups.ResourceGroupId == resourcegrp.ResourceGroupId)
                            {
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
                                    ResourceGroup = new ResourceGroupsSchema { ResourceGroupId = -1, GroupName = "", Position = -1 },
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
                                    RelatedTaxTags = article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags"),
                                    RelatedArticles = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles"),
                                    RelatedResources = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedResources"),
                                    Discriminator = article.GetPropertyValue<string>("Discriminator"),
                                    PartitionKey = ""
                                };
                                await _Eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                            }
                        } }
                        foreach (var doc in resourcegrpDocs.Where(d => d.GetPropertyValue<int>("ResourceGroupId") == resourcegrp.ResourceGroupId))
                        {
                            var resourceEvent = new ResourceGroupCommandEvent()
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Delete,
                                Discriminator = Constants.ResourceGroupsDiscriminator,
                                PartitionKey = doc.GetPropertyValue<int>("LanguageId").ToString()
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(resourceEvent);
                        }
                    }
                }
                scope.Complete();
            }
            return response;
        }
    }
}
