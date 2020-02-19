//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
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
using System.Transactions;
using Author.Core.Framework;

namespace Author.Command.Service
{
   public class CreateCountryGroupsCommandHandler : IRequestHandler<CreateCountryGroupsCommand, CreateCountryGroupsCommandResponse>
    {
        private readonly IIntegrationEventBlobService _Eventblobcontext;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryGroupsRepository _CountryGroupsRepository;
        private readonly ILogger _logger;

        public CreateCountryGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateCountryGroupsCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext)
        {
            _CountryGroupsRepository = new CountryGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
        }
        public async Task<CreateCountryGroupsCommandResponse> Handle(CreateCountryGroupsCommand request, CancellationToken cancellationToken)
        {
            CreateCountryGroupsCommandResponse response = new CreateCountryGroupsCommandResponse()
            {
                IsSuccessful = false
            };

            CountryGroups _countryGroup = new CountryGroups();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
             //   List<Languages> _languages = _ResourceGroupRepository.GetAllLanguages();
                _countryGroup.IsPublished = true;
                _countryGroup.CreatedBy = "";
                _countryGroup.CreatedDate = DateTime.Now;
                _countryGroup.UpdatedBy = "";
                _countryGroup.UpdatedDate = DateTime.Now;
                foreach (var langName in request.LanguageName)
                {
                    var countryGroupContent = new CountryGroupContents();
                    countryGroupContent.GroupName = langName.Name.Trim();
                    countryGroupContent.LanguageId = langName.LanguageId;
                    _countryGroup.CountryGroupContents.Add(countryGroupContent);
                }
                foreach (var countryId in request.CountryIds)
                {
                    var countryGroupCountriesContent = new CountryGroupAssociatedCountries();
                    countryGroupCountriesContent.CountryId = countryId;
                    _countryGroup.CountryGroupAssociatedCountries.Add(countryGroupCountriesContent);
                }
                _CountryGroupsRepository.Add(_countryGroup);
                await _CountryGroupsRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var content in _countryGroup.CountryGroupContents)
                {
                    var eventSourcing = new CountryGroupCommandEvent()
                    {
                        EventType = ServiceBusEventType.Create,
                        CountryGroupId = _countryGroup.CountryGroupId,
                        IsPublished = _countryGroup.IsPublished,
                        CreatedBy = _countryGroup.CreatedBy,
                        CreatedDate = _countryGroup.CreatedDate,
                        UpdatedBy = _countryGroup.UpdatedBy,
                        UpdatedDate = _countryGroup.UpdatedDate,
                        GroupName = content.GroupName,
                        CountryGroupContentId = content.CountryGroupContentId,
                        LanguageId = content.LanguageId,
                        AssociatedCountryIds = (from cg in _countryGroup.CountryGroupAssociatedCountries where cg != null select cg.CountryId).ToList(),
                        Discriminator = Constants.CountryGroupsDiscriminator,
                        PartitionKey = ""
                    };
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
