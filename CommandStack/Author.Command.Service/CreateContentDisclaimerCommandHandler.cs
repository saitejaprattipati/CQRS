using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
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

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var contentDisclaimer = _mapper.Map<Disclaimers>(request);
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

            return response;
        }
    }
}
