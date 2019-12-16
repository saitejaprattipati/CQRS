using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework.ExceptionHandling;
using Author.Core.Framework;
using AutoMapper;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class UpdateSystemUserCommandHandler : IRequestHandler<UpdateSystemUserCommand, UpdateSystemUserCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SystemUserRepository _systemUserRepository;
        private readonly IMapper _mapper;


        public UpdateSystemUserCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _systemUserRepository = new SystemUserRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _mapper = mapper;
        }

        public async Task<UpdateSystemUserCommandResponse> Handle(UpdateSystemUserCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateSystemUserCommandResponse()
            {
                IsSuccessful = false
            };

            var userExists = _systemUserRepository.UserExists(request.Email, request.SystemUserId);

            if (!userExists)
            {
                throw new RulesException("email", $"User with SystemuserId: {request.SystemUserId} and email address: {request.Email} does not exists");
            }

            var user = _mapper.Map<SystemUsers>(request);
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
                user.UpdatedBy = "CMS Admin";
                user.UpdatedDate = DateTime.UtcNow;
                _systemUserRepository.Update(user);
                await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();

                var isExistingSysUserCountriesRemoved = await _systemUserRepository.RemoveSystemUserAssociatedCountriesAsync(Convert.ToInt32(request.SystemUserId));

                if (isExistingSysUserCountriesRemoved)
                {
                    await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();
                }

                var homeCountry = new SystemUserAssociatedCountries();
                homeCountry.CountryId = request.HomeCountry;
                homeCountry.IsPrimary = true;
                homeCountry.SystemUserId = user.SystemUserId;
                _systemUserRepository.Add(homeCountry);

                foreach (var country in request.Countries.Where(x => !x.Equals(request.HomeCountry)))
                {
                    var associatedCountry = new SystemUserAssociatedCountries();
                    associatedCountry.CountryId = country;
                    associatedCountry.SystemUserId = user.SystemUserId;
                    associatedCountry.IsPrimary = false;
                    _systemUserRepository.Add(associatedCountry);
                }

                await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            foreach (var content in user.SystemUserAssociatedCountries)
            {
                var eventSourcing = new SystemUserCommandEvent()
                {
                    EventType = (int)ServiceBusEventType.Update,
                    Discriminator = Constants.SystemUsersDiscriminator,
                    SystemUserId = user.SystemUserId,
                    CreatedBy = user.CreatedBy,
                    CreatedDate = user.CreatedDate,
                    UpdatedBy = user.UpdatedBy,
                    UpdatedDate = user.UpdatedDate,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    WorkPhoneNumber = user.WorkPhoneNumber,
                    MobilePhoneNumber = user.MobilePhoneNumber,
                    Level = user.Level,
                    Role = user.Role,
                    Location = user.Location,
                    Email = user.Email,
                    TimeZone = user.TimeZone,
                    CountryId = content.CountryId,
                    IsPrimary = content.IsPrimary,
                    SystemUserAssociatedCountryId = content.SystemUserAssociatedCountryId
                };
                await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
            }
            return response;
        }
    }
}
