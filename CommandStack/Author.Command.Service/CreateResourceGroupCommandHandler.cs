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
                List<Languages> _languages=_ResourceGroupRepository.GetAllLanguages();
                ResourceGroups _resourceGroup = new ResourceGroups();
                _resourceGroup.IsPublished=true;
                _resourceGroup.Position = request.Position;
                _resourceGroup.CreatedBy = "";
                _resourceGroup.CreatedDate = DateTime.Now;
                _resourceGroup.UpdatedBy = "";
                _resourceGroup.UpdatedDate = DateTime.Now;
                foreach (var langName in request.languageName)
                {
                    var resourceGroupContent = new ResourceGroupContents();
                    resourceGroupContent.GroupName = langName.Name.Trim();
                    resourceGroupContent.LanguageId = Convert.ToInt32(_languages.FirstOrDefault(x => x.Locale == langName.Language).LanguageId.ToString());
                    _resourceGroup.ResourceGroupContents.Add(resourceGroupContent);
                }
                _ResourceGroupRepository.Add(_resourceGroup);        
                await _ResourceGroupRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                return response;
        }
    }
}
