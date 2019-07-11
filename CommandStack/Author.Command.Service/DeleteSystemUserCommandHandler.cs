using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework.ExceptionHandling;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class DeleteSystemUserCommandHandler:IRequestHandler<DeleteSystemUserCommand, DeleteSystemUserCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SystemUserRepository _systemUserRepository;

        public DeleteSystemUserCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _systemUserRepository = new SystemUserRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
        }

        public async Task<DeleteSystemUserCommandResponse> Handle(DeleteSystemUserCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteSystemUserCommandResponse()
            {
                IsSuccessful = false
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var systemUserIds = request.SystemUserIds.Distinct().ToList();
                var systemusers = await _systemUserRepository.GetSystemUsersByIds(systemUserIds);
                if (systemusers.Count != systemUserIds.Count)
                {
                    throw new RulesException("Invalid", @"User not found");
                }

                foreach (var sysuser in systemusers)
                {
                    foreach (var sysuserassociatedcountry in sysuser.SystemUserAssociatedCountries.ToList())
                    {
                        sysuser.SystemUserAssociatedCountries.Remove(sysuserassociatedcountry);
                        _systemUserRepository.Delete(sysuserassociatedcountry);
                    }
                    _systemUserRepository.DeleteSystemUser(sysuser);
                }

                await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();

                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
