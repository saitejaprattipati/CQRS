using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Framework.ExceptionHandling;
using Author.Core.Services.Persistence.CosmosDB;
using MediatR;
using System;
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
        private readonly CosmosDBContext _context;

        public DeleteSystemUserCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _systemUserRepository = new SystemUserRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _context = new CosmosDBContext();
        }

        public async Task<DeleteSystemUserCommandResponse> Handle(DeleteSystemUserCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteSystemUserCommandResponse()
            {
                IsSuccessful = false
            };

            var systemUserIds = request.SystemUserIds.Distinct().ToList();
            var systemusers = await _systemUserRepository.GetSystemUsersByIds(systemUserIds);
            if (systemusers.Count != systemUserIds.Count)
            {
                throw new RulesException("Invalid", @"User not found");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
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
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var userDocs = _context.GetAll(Constants.SystemUsersDiscriminator);
                foreach (var user in systemusers)
                {
                    //foreach (var country in user.SystemUserAssociatedCountries)
                    //{
                        var eventsourcing = new SystemUserCommandEvent()
                        {
                            id = userDocs.FirstOrDefault(d => d.GetPropertyValue<int>("SystemUserId") == user.SystemUserId).GetPropertyValue<Guid>("id"),
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.SystemUsersDiscriminator,
                            SystemUserId = user.SystemUserId
                        };
                        await _eventcontext.PublishThroughEventBusAsync(eventsourcing);
                    //}
                }
                scope.Complete();
            }
            return response;
        }
    }
}
