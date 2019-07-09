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
        private readonly ResourceGroupRepository _systemUserRepository;
        private readonly IMapper _mapper;

        public UpdateResourceGroupCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _systemUserRepository = new ResourceGroupRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _mapper = mapper;
        }
        public async Task<UpdateResourceGroupCommandResponse> Handle(UpdateResourceGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateResourceGroupCommandResponse()
            {
                IsSuccessful = false
            };

           var resourceGroup = _systemUserRepository.GetResourceGroup(request.ResourceGroupId);
            resourceGroup.Position = request.Position;
            List<Languages> languages= _systemUserRepository.GetAllLanguages();

            foreach (var content in request.LanguageName)
            {
                var resourceGroupContents = resourceGroup.ResourceGroupContents.Where(s => s.LanguageId == (languages.Where(x => x.Locale == content.Language).Select(a=>a.LanguageId).FirstOrDefault())).FirstOrDefault();
                if (resourceGroupContents==null)
                {
                    ResourceGroupContents objresourceGroupContents = new ResourceGroupContents();
                    objresourceGroupContents.GroupName = content.Name;
                    objresourceGroupContents.LanguageId = (languages.Where(x => x.Locale == content.Language).Select(a => a.LanguageId).FirstOrDefault());
                    resourceGroup.ResourceGroupContents.Add(objresourceGroupContents);
                }
                else
                {
                    resourceGroupContents.GroupName = content.Name;
                    _systemUserRepository.Update(resourceGroupContents);
                }
            }

            foreach(var content in resourceGroup.ResourceGroupContents.Where(s=>s.ResourceGroupId==request.ResourceGroupId))
            {
                if(request.LanguageName.Where(s=>s.Language== (languages.Where(x => x.LanguageId == content.LanguageId).Select(a => a.Locale).FirstOrDefault()))==null)
                {
                    resourceGroup.ResourceGroupContents.Remove(content);
                    _systemUserRepository.Delete(content);
                }
            }
            resourceGroup.UpdatedBy = "";
            resourceGroup.UpdatedDate = DateTime.Now;
            return response;
        }
    }
}
