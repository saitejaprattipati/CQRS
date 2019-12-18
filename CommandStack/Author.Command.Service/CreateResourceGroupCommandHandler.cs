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
   public class CreateResourceGroupCommandHandler : IRequestHandler<CreateResourceGroupCommand, CreateResourceGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ResourceGroupRepository _ResourceGroupRepository;
          private readonly ILogger _logger;

        public CreateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateResourceGroupCommandHandler> logger)
        {
            _ResourceGroupRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
                _logger = logger;
        }
        
        public async Task<CreateResourceGroupCommandResponse> Handle(CreateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            CreateResourceGroupCommandResponse response = new CreateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };
            ResourceGroups _resourceGroup = new ResourceGroups();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //List<Languages> _languages = _ResourceGroupRepository.GetAllLanguages();
                _resourceGroup.IsPublished = true;
                _resourceGroup.Position = request.Position;
                _resourceGroup.CreatedBy = "";
                _resourceGroup.CreatedDate = DateTime.Now;
                _resourceGroup.UpdatedBy = "";
                _resourceGroup.UpdatedDate = DateTime.Now;
                foreach (var langName in request.languageName)
                {
                    var resourceGroupContent = new ResourceGroupContents();
                    resourceGroupContent.GroupName = langName.Name.Trim();
                    resourceGroupContent.LanguageId = langName.LanguageId;
                    _resourceGroup.ResourceGroupContents.Add(resourceGroupContent);
                }
                _ResourceGroupRepository.Add(_resourceGroup);
                await _ResourceGroupRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            foreach(var contnet in _resourceGroup.ResourceGroupContents)
            {
                var eventSourcing = new ResourceGroupCommandEvent()
                {
                    EventType = (int)ServiceBusEventType.Create,
                    Discriminator = Constants.ResourceGroupsDiscriminator,
                    ResourceGroupId = _resourceGroup.ResourceGroupId,
                    IsPublished = _resourceGroup.IsPublished,
                    CreatedBy = _resourceGroup.CreatedBy,
                    CreatedDate = _resourceGroup.CreatedDate,
                    UpdatedBy = _resourceGroup.UpdatedBy,
                    UpdatedDate = _resourceGroup.UpdatedDate,
                    Position = _resourceGroup.Position,
                    ResourceGroupContentId = contnet.ResourceGroupContentId,
                    LanguageId = contnet.LanguageId,
                    GroupName = contnet.GroupName
                };
                await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
            }
            return response;
        }
    }
}
