using Author.Command.Domain.Command;
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

namespace Author.Command.Service
{
    class UpdateResourceGroupCommandHandler : IRequestHandler<UpdateResourceGroupCommand, UpdateResourceGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly ResourceGroupRepository _ResourceGroupRepository;
        private readonly IMapper _mapper;

        public UpdateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _ResourceGroupRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _mapper = mapper;
        }
        public async Task<UpdateResourceGroupCommandResponse> Handle(UpdateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<int> objresourceGroupId = new List<int>();
                objresourceGroupId.Add(request.ResourceGroupId);
                //test
                var resourceGroup = _ResourceGroupRepository.getResourceGroups(objresourceGroupId)[0];
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
            return response;
        }
    }
}
