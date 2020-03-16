using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework.ExceptionHandling;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;
using Author.Command.Domain.Models;

namespace Author.Command.Service
{
    class UpdateResourceGroupCommandHandler : IRequestHandler<UpdateResourceGroupCommand, UpdateResourceGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly ResourceGroupRepository _ResourceGroupRepository;
        private readonly IMapper _mapper;
        private readonly CosmosDBContext _context;

        public UpdateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _ResourceGroupRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _mapper = mapper;
            _context = new CosmosDBContext();
        }
        public async Task<UpdateResourceGroupCommandResponse> Handle(UpdateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };
            List<int> objresourceGroupId = new List<int>();
            objresourceGroupId.Add(request.ResourceGroupId);
            var resourceGroup = _ResourceGroupRepository.getResourceGroups(objresourceGroupId)[0];
            var contentToDelete = new List<int>();
            var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
            var resourceGroupsDocs = _context.GetAll(Constants.ResourceGroupsDiscriminator);
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                resourceGroup.Position = request.Position;
                //List<Languages> languages = _ResourceGroupRepository.GetAllLanguages();

                foreach (var content in request.LanguageName)
                {
                    var resourceGroupContents = resourceGroup.ResourceGroupContents.Where(s => s.LanguageId == content.LanguageId).FirstOrDefault();
                    if (resourceGroupContents == null)
                    {
                        ResourceGroupContents objresourceGroupContents = new ResourceGroupContents();
                        objresourceGroupContents.GroupName = content.Name;
                        objresourceGroupContents.LanguageId = content.LanguageId;
                        resourceGroup.ResourceGroupContents.Add(objresourceGroupContents);
                    }
                    else
                    {
                        resourceGroupContents.GroupName = content.Name;
                        _ResourceGroupRepository.Update(resourceGroupContents);
                    }
                }
                //    List<ResourceGroupContents> ResourceGroupContents = resourceGroup.ResourceGroupContents.Where(s => s.ResourceGroupId == request.ResourceGroupId).ToList();
                foreach (var resourceContent in resourceGroup.ResourceGroupContents.ToList())
                {
                    if (request.LanguageName.Where(s => s.LanguageId == resourceContent.LanguageId).Count() == 0)
                    {
                        contentToDelete.Add((int)resourceContent.LanguageId);
                        resourceGroup.ResourceGroupContents.Remove(resourceContent);
                        _ResourceGroupRepository.Delete(resourceContent);
                    }
                }
                resourceGroup.UpdatedBy = "";
                resourceGroup.UpdatedDate = DateTime.Now;
                await _ResourceGroupRepository.UnitOfWork
                      .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var resourcegroupDocs = _context.GetAll(Constants.ResourceGroupsDiscriminator);
                foreach (var content in resourceGroup.ResourceGroupContents)
                {
                    foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == content.LanguageId))
                    {
                        // foreach (var relatedResourceGroups in article.GetPropertyValue<List<ResourceGroupsSchema>>("ResourceGroup"))
                        //{
                        var relatedResourceGroups = article.GetPropertyValue<List<ResourceGroupsSchema>>("ResourceGroup").FirstOrDefault();
                        if (relatedResourceGroups.ResourceGroupId == resourceGroup.ResourceGroupId)
                        {
                            ResourceGroupsSchema resourceGroupsSchema = new ResourceGroupsSchema();
                            //relatedArticleSchema = article.GetPropertyValue<ResourceGroupsSchema>("RelatedArticles");

                            // var index = relatedArticleSchema.IndexOf(relatedArticleSchema.Where(i => i.ArticleId == _article.ArticleId).First());
                            //if (index != -1)
                            resourceGroupsSchema = new ResourceGroupsSchema { ResourceGroupId = resourceGroup.ResourceGroupId, GroupName = content.GroupName == null ? "" : content.GroupName, Position = resourceGroup.Position == null ? -1 : resourceGroup.Position };
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
                                ResourceGroup = resourceGroupsSchema,
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
                            await _eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                        }

                    }
                    var doc = resourcegroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ResourceGroupId") == resourceGroup.ResourceGroupId
                                                  && d.GetPropertyValue<int?>("LanguageId") == content.LanguageId);
                    var eventSourcing = new ResourceGroupCommandEvent()
                    {
                        id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                        EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                        Discriminator = Constants.ResourceGroupsDiscriminator,
                        ResourceGroupId = resourceGroup.ResourceGroupId,
                        IsPublished = resourceGroup.IsPublished,
                        CreatedBy = resourceGroup.CreatedBy,
                        CreatedDate = resourceGroup.CreatedDate,
                        UpdatedBy = resourceGroup.UpdatedBy,
                        UpdatedDate = resourceGroup.UpdatedDate,
                        Position = resourceGroup.Position,
                        ResourceGroupContentId = content.ResourceGroupContentId,
                        LanguageId = content.LanguageId,
                        GroupName = content.GroupName,
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach (int i in contentToDelete)
                {
                    foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == i))
                    {
                        var relatedResourceGroups = article.GetPropertyValue<List<ResourceGroupsSchema>>("ResourceGroup").FirstOrDefault();
                        if (relatedResourceGroups.ResourceGroupId == resourceGroup.ResourceGroupId)
                        {
                            var GroupNameInEnglish = resourceGroupsDocs.Where(rg => rg.GetPropertyValue<int>("ResourceGroupContentId") == resourceGroup.ResourceGroupId && rg.GetPropertyValue<int>("LanguageId") == 37).Select(rgs => rgs.GetPropertyValue<string>("GroupName")).FirstOrDefault();
                            var ResourceGroup = (GroupNameInEnglish == "") ? new ResourceGroupsSchema { ResourceGroupId = -1, GroupName = "", Position = -1 } : new ResourceGroupsSchema { ResourceGroupId = resourceGroup.ResourceGroupId, GroupName = GroupNameInEnglish, Position = relatedResourceGroups.Position };
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
                            await _eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                        }
                    }
                    var deleteEvt = new ResourceGroupCommandEvent()
                    {
                        id = resourcegroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ResourceGroupId") == resourceGroup.ResourceGroupId
                                     && d.GetPropertyValue<int?>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.ResourceGroupsDiscriminator,
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(deleteEvt);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
