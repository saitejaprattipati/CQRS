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
using Author.Core.Framework.ExceptionHandling;
using System.Transactions;

namespace Author.Command.Service
{
   public class ManipulateCountriesGroupsCommandHandler : IRequestHandler<ManipulateCountryGroupsCommand, ManipulateCountryGroupsCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryGroupsRepository _CountryGroupsRepository;
        private readonly ILogger _logger;
        public ManipulateCountriesGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateCountriesGroupsCommandHandler> logger)
        {
            _CountryGroupsRepository = new CountryGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }

        public async Task<ManipulateCountryGroupsCommandResponse> Handle(ManipulateCountryGroupsCommand request, CancellationToken cancellationToken)
        {
            ManipulateCountryGroupsCommandResponse response = new ManipulateCountryGroupsCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<CountryGroups> countryGroups = _CountryGroupsRepository.getCountryGroups(request.CountryGroupIds);
                if (request.CountryGroupIds.Count != countryGroups.Count)
                    throw new RulesException("Invalid", @"CountryGroup not found");

                if (request.Operation == "Publish")
                {
                    foreach (var countryGroup in countryGroups)
                    {
                        countryGroup.IsPublished = true;
                        _CountryGroupsRepository.Update<CountryGroups>(countryGroup);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var countryGroup in countryGroups)
                    {
                        countryGroup.IsPublished = false;
                        _CountryGroupsRepository.Update<CountryGroups>(countryGroup);
                    }
                }
                else if (request.Operation == "Delete")
                {

                    foreach (CountryGroups countryGroup in countryGroups)
                    {
                        foreach (var countryCountry in countryGroup.CountryGroupAssociatedCountries.ToList())
                        {
                            countryGroup.CountryGroupAssociatedCountries.Remove(countryCountry);
                            _CountryGroupsRepository.Delete<CountryGroupAssociatedCountries>(countryCountry);
                        }

                        foreach (var countryContents in countryGroup.CountryGroupContents.ToList())
                        {
                            countryGroup.CountryGroupContents.Remove(countryContents);
                            _CountryGroupsRepository.Delete<CountryGroupContents>(countryContents);
                        }
                        _CountryGroupsRepository.DeleteCountryGroup(countryGroup);
                    }

                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _CountryGroupsRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
