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
using Author.Core.Services.Persistence.CosmosDB;
using System.Collections.Generic;

namespace Author.Command.Service
{
    public class UpdateSystemUserCommandHandler : IRequestHandler<UpdateSystemUserCommand, UpdateSystemUserCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SystemUserRepository _systemUserRepository;
        private readonly IMapper _mapper;
        private readonly CosmosDBContext _context;

        public UpdateSystemUserCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _systemUserRepository = new SystemUserRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _mapper = mapper;
            _context = new CosmosDBContext();
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

            //var user = _mapper.Map<SystemUsers>(request);
            var user = (await _systemUserRepository.GetSystemUsersByIds(new List<int> { request.SystemUserId })).FirstOrDefault();
            var countriesToDelete = new List<int>();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Level = request.Level;
                user.Role = request.Role;
                user.WorkPhoneNumber = request.WorkPhoneNumber;
                user.MobilePhoneNumber = request.MobilePhoneNumber;
                user.Location = request.Location;
                user.Email = request.Email;
                user.UpdatedBy = "CMS Admin";
                user.UpdatedDate = DateTime.UtcNow;

                var homeCountry = user.SystemUserAssociatedCountries.FirstOrDefault(c => c.CountryId == request.HomeCountry);
                if (homeCountry == null)
                {
                    homeCountry = new SystemUserAssociatedCountries
                    {
                        CountryId = request.HomeCountry,
                        IsPrimary = true,
                        SystemUserId = user.SystemUserId
                    };
                    _systemUserRepository.Add(homeCountry);
                }
                else
                {
                    homeCountry.IsPrimary = true;
                    homeCountry.SystemUserId = user.SystemUserId;
                    homeCountry.CountryId = request.HomeCountry;
                    _systemUserRepository.Update(homeCountry);
                }

                foreach (var country in request.Countries.Where(x => !x.Equals(request.HomeCountry)))
                {
                    if (user.SystemUserAssociatedCountries.FirstOrDefault(c => c.CountryId == country) == null)
                    {
                        var associatedCountries = new SystemUserAssociatedCountries
                        {
                            CountryId = country,
                            SystemUserId = user.SystemUserId,
                            IsPrimary = false
                        };
                        user.SystemUserAssociatedCountries.Add(associatedCountries);
                        _systemUserRepository.Add(associatedCountries);
                    }
                }
                foreach(var associatedCountry in user.SystemUserAssociatedCountries.ToList())
                {
                    if (request.Countries.Where(c => c == associatedCountry.CountryId).Count() == 0)
                    {
                        countriesToDelete.Add(associatedCountry.SystemUserAssociatedCountryId);
                        user.SystemUserAssociatedCountries.Remove(associatedCountry);
                        _systemUserRepository.Delete(associatedCountry);
                    }
                }

                await _systemUserRepository.UnitOfWork.SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var systemuserDocs = _context.GetAll(Constants.SystemUsersDiscriminator);
                foreach (var content in user.SystemUserAssociatedCountries)
                {
                    var doc = systemuserDocs.FirstOrDefault(d => d.GetPropertyValue<int>("SystemUserId") == user.SystemUserId
                              && d.GetPropertyValue<int>("SystemUserAssociatedCountryId") == content.SystemUserAssociatedCountryId);
                    var eventSourcing = new SystemUserCommandEvent()
                    {
                        id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                        EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                        Discriminator = Constants.SystemUsersDiscriminator,
                        SystemUserId = user.SystemUserId,
                        CreatedBy = user.CreatedBy ?? string.Empty,
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
                foreach(int i in countriesToDelete)
                {
                    var deleteEvt = new SystemUserCommandEvent()
                    {
                        id = systemuserDocs.FirstOrDefault(d => d.GetPropertyValue<int>("SystemUserId") == user.SystemUserId
                              && d.GetPropertyValue<int>("SystemUserAssociatedCountryId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.SystemUsersDiscriminator
                    };
                    await _eventcontext.PublishThroughEventBusAsync(deleteEvt);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
