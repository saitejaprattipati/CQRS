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
                foreach (var contnet in resourceGroup.ResourceGroupContents)
                {
                    var doc = resourcegroupDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ResourceGroupId") == resourceGroup.ResourceGroupId
                                                  && d.GetPropertyValue<int?>("LanguageId") == contnet.LanguageId);
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
                        ResourceGroupContentId = contnet.ResourceGroupContentId,
                        LanguageId = contnet.LanguageId,
                        GroupName = contnet.GroupName,
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach(int i in contentToDelete)
                {
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
