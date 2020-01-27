using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class CreateContentDisclaimerCommandHandler : IRequestHandler<CreateContentDisclaimerCommand, CreateContentDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly IMapper _mapper;
        private readonly ContentDisclaimerRepository _contentDisclaimerRepository;

        public CreateContentDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _eventcontext = eventcontext;
            _contentDisclaimerRepository = new ContentDisclaimerRepository(new TaxatHand_StgContext());
            _mapper = mapper;
        }

        public async Task<CreateContentDisclaimerCommandResponse> Handle(CreateContentDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateContentDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            var contentDisclaimer = _mapper.Map<Disclaimers>(request);
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                contentDisclaimer.CreatedBy = "CMS Admin";
                contentDisclaimer.CreatedDate = DateTime.UtcNow;
                contentDisclaimer.UpdatedBy = "CMS Admin";
                contentDisclaimer.UpdatedDate = DateTime.UtcNow;

                await _contentDisclaimerRepository.AddAsync(contentDisclaimer);

                await _contentDisclaimerRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var content in contentDisclaimer.DisclaimerContents)
                {
                    var eventSourcing = new DisclaimerCommandEvent()
                    {
                        EventType = ServiceBusEventType.Create,
                        Name = contentDisclaimer.Name,
                        DisclaimerId = contentDisclaimer.DisclaimerId,
                        CreatedBy = contentDisclaimer.CreatedBy,
                        CreatedDate = contentDisclaimer.CreatedDate,
                        UpdatedBy = contentDisclaimer.UpdatedBy,
                        UpdatedDate = contentDisclaimer.UpdatedDate,
                        DefaultCountryId = contentDisclaimer.DefaultCountryId,
                        ProviderName = content.ProviderName ?? string.Empty,
                        ProviderTerms = content.ProviderTerms ?? string.Empty,
                        LanguageId = content.LanguageId,
                        DisclaimerContentId = content.DisclaimerContentId,
                        Discriminator = Constants.DisclaimersDiscriminator
                    };
                    await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                scope.Complete();
            }

            return response;
        }
    }
}
