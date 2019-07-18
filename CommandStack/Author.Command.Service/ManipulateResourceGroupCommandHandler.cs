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

namespace Author.Command.Service
{
    public class ManipulateResourceGroupCommandHandler : IRequestHandler<ManipulateResourceGroupCommand, ManipulateResourceGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ResourceGroupRepository _ResourceGroupRepository;
        private readonly ILogger _logger;
        public ManipulateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateResourceGroupCommandHandler> logger)
        {
            _ResourceGroupRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }

        public async Task<ManipulateResourceGroupCommandResponse> Handle(ManipulateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            ManipulateResourceGroupCommandResponse response = new ManipulateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<ResourceGroups> resourceGroups = _ResourceGroupRepository.getResourceGroups(request.ResourceGroupIds);
                if (request.ResourceGroupIds.Count != resourceGroups.Count)
                    throw new RulesException("Invalid", @"ResourceGroup not found");

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
            return response;
        }
    }
}
