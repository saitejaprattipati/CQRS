using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework.ExceptionHandling;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class CreateUserCommandHandler : IRequestHandler<SystemUserViewCommand, CreateSystemUserCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SystemUserRepository _systemUserRepository;

        public CreateUserCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _systemUserRepository = new SystemUserRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
        }
        public async Task<CreateSystemUserCommandResponse> Handle(SystemUserViewCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateSystemUserCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Check User Exists
                var userExists = _systemUserRepository.UserExists(request.Email);
                if (userExists)
                {
                    //ModelState.AddModelError("email", new Exception("This email address already exists"));
                    //throw new HttpStatusCodeException(StatusCodes.Status400BadRequest, @"This email address already exists");
                    throw new RulesException("email", @"This email address already exists");
                }

                var user = new SystemUsers
                {
                    FirstName = request.FirstName,
                    Email = request.Email,
                    LastName = request.LastName,
                    Level = request.Level,
                    Location = request.Location,
                    MobilePhoneNumber = request.MobilePhoneNumber,
                    Role = Convert.ToInt32(request.Role),
                    WorkPhoneNumber = request.WorkPhoneNumber,
                    CreatedBy = "CMS Admin",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "CMS Admin",
                    UpdatedDate = DateTime.UtcNow
                };

                _systemUserRepository.Add(user);
                await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();


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
            return response;
        }
    }
}
