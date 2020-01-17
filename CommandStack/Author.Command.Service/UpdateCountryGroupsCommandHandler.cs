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
using Author.Core.Services.Persistence.CosmosDB;

namespace Author.Command.Service
{
   public class UpdateCountryGroupsCommandHandler : IRequestHandler<UpdateCountryGroupsCommand, UpdateCountryGroupsCommandResponse>
    {
        private readonly IIntegrationEventBlobService _Eventblobcontext;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly CountryGroupsRepository _CountryGroupsRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;

        public UpdateCountryGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<UpdateCountryGroupsCommandHandler> logger, IIntegrationEventBlobService Eventblobcontext)
        {
            _CountryGroupsRepository = new CountryGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _Eventblobcontext = Eventblobcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }
        public async Task<UpdateCountryGroupsCommandResponse> Handle(UpdateCountryGroupsCommand request, CancellationToken cancellationToken)
        {
            UpdateCountryGroupsCommandResponse response = new UpdateCountryGroupsCommandResponse()
            {
                IsSuccessful = false
            };

            List<int> objresourceGroupId = new List<int>();
            objresourceGroupId.Add(request.CountryGroupsId);
            var countryGroup = _CountryGroupsRepository.getCountryGroups(objresourceGroupId)[0];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                countryGroup.CreatedBy = "";
                countryGroup.CreatedDate = DateTime.Now;
                countryGroup.UpdatedBy = "";
                countryGroup.UpdatedDate = DateTime.Now;

                foreach (var content in request.LanguageName)
                {
                    var countryGroupContents = countryGroup.CountryGroupContents.Where(s => s.LanguageId == content.LanguageId).FirstOrDefault();
                    if (countryGroupContents == null)
                    {
                        CountryGroupContents objresourceGroupContents = new CountryGroupContents();
                        objresourceGroupContents.GroupName = content.Name;
                        objresourceGroupContents.LanguageId = content.LanguageId;
                        countryGroup.CountryGroupContents.Add(objresourceGroupContents);
                    }
                    else
                    {
                        countryGroupContents.GroupName = content.Name;
                        _CountryGroupsRepository.Update(countryGroupContents);
                    }
                }
                //var countryGroupCountries = countryGroup.CountryGroupAssociatedCountries.Where(s => s.CountryGroupId == request.CountryGroupsId);
                foreach (var content in request.CountryIds)
                {
                    if (countryGroup.CountryGroupAssociatedCountries.Where(s => s.CountryId == content).FirstOrDefault() == null)
                    {
                        CountryGroupAssociatedCountries objcountryGroupCountries = new CountryGroupAssociatedCountries();
                        objcountryGroupCountries.CountryId = content;
                        countryGroup.CountryGroupAssociatedCountries.Add(objcountryGroupCountries);
                    }
                }
                //    List<ResourceGroupContents> ResourceGroupContents = resourceGroup.ResourceGroupContents.Where(s => s.ResourceGroupId == request.ResourceGroupId).ToList();
                foreach (var countryGroupContent in countryGroup.CountryGroupContents.ToList())
                {
                    if (request.LanguageName.Where(s => s.LanguageId == countryGroupContent.LanguageId).Count() == 0)
                    {
                        countryGroup.CountryGroupContents.Remove(countryGroupContent);
                        _CountryGroupsRepository.Delete(countryGroupContent);
                    }
                }
                foreach (var countryGroupCountries in countryGroup.CountryGroupAssociatedCountries.ToList())
                {
                    if (request.CountryIds.Where(s => s== countryGroupCountries.CountryId).Count() == 0)
                    {
                        countryGroup.CountryGroupAssociatedCountries.Remove(countryGroupCountries);
                        _CountryGroupsRepository.Delete(countryGroupCountries);
                    }
                }
                countryGroup.UpdatedBy = "";
                countryGroup.UpdatedDate = DateTime.Now;
                await _CountryGroupsRepository.UnitOfWork
                      .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            var countryGroupDocs = _context.GetAll(Constants.CountryGroupsDiscriminator).Records as IEnumerable<CountryGroupCommandEvent>;
            foreach (var content in countryGroup.CountryGroupContents)
            {
                var eventSourcing = new CountryGroupCommandEvent()
                {
                    id = countryGroupDocs.ToList().Find(d => d.CountryGroupId == countryGroup.CountryGroupId && d.LanguageId == content.LanguageId).id,
                    EventType = ServiceBusEventType.Update,
                    CountryGroupId = countryGroup.CountryGroupId,
                    IsPublished = countryGroup.IsPublished,
                    CreatedBy = countryGroup.CreatedBy,
                    CreatedDate = countryGroup.CreatedDate,
                    UpdatedBy = countryGroup.UpdatedBy,
                    UpdatedDate = countryGroup.UpdatedDate,
                    GroupName = content.GroupName,
                    CountryGroupContentId = content.CountryGroupContentId,
                    LanguageId = content.LanguageId,
                    AssociatedCountryIds = (from cg in countryGroup.CountryGroupAssociatedCountries where cg != null select cg.CountryId).ToList(),
                    Discriminator = Constants.CountryGroupsDiscriminator
                };
                await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
            }
            return response;
        }
    }
}
