using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Framework.ExceptionHandling;
using Author.Core.Services.Persistence.CosmosDB;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class UpdateContentDisclaimerCommandHandler : IRequestHandler<UpdateContentDisclaimerCommand, UpdateContentDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly IMapper _mapper;
        private readonly ContentDisclaimerRepository _contentDisclaimerRepository;
        private readonly CosmosDBContext _context;

        public UpdateContentDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _eventcontext = eventcontext;
            _contentDisclaimerRepository = new ContentDisclaimerRepository(new TaxatHand_StgContext());
            _mapper = mapper;
            _context = new CosmosDBContext();
        }

        public async Task<UpdateContentDisclaimerCommandResponse> Handle(UpdateContentDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateContentDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };
            var disclaimer = await _contentDisclaimerRepository.GetContentDisclaimer(request.DisclaimerId);

            if (disclaimer == null)
            {
                throw new RulesException("contentDisclaimer", $"ContentDisclaimer with DisclaimerId: {request.DisclaimerId}  not found");
            }

            var contentToDelete = new List<int>();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //Update existing Content disclaimer

                disclaimer.Name = request.GroupName;
                disclaimer.UpdatedBy = "CMS Admin";
                disclaimer.UpdatedDate = DateTime.UtcNow;
                disclaimer.DefaultCountryId = request.DefaultCountryId;

                foreach (var item in request.DisclaimerContent)
                {
                    var disclaimerContent = disclaimer.DisclaimerContents.FirstOrDefault(x => x.LanguageId.Equals(item.LanguageId));
                    if (disclaimerContent == null)
                    {
                        disclaimerContent = new DisclaimerContents
                        {
                            ProviderName = item.ProviderName,
                            ProviderTerms = item.ProviderTerms,
                            LanguageId = item.LanguageId
                        };

                        disclaimer.DisclaimerContents.Add(disclaimerContent);
                    }
                    else
                    {
                        disclaimerContent.ProviderName = item.ProviderName;
                        disclaimerContent.ProviderTerms = item.ProviderTerms;
                        disclaimerContent.LanguageId = item.LanguageId;
                        _contentDisclaimerRepository.Update(disclaimerContent);
                    }
                }

                foreach (var item in disclaimer.DisclaimerContents.ToList())
                {
                    if (request.DisclaimerContent.Where(dc => dc.LanguageId.Equals(item.LanguageId)).Count() == 0)
                    {
                        contentToDelete.Add((int)item.LanguageId);
                        disclaimer.DisclaimerContents.Remove(item);
                        _contentDisclaimerRepository.Delete(item);
                    }
                }

                await _contentDisclaimerRepository.UnitOfWork.SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var disclaimerDocs = _context.GetAll(Constants.DisclaimersDiscriminator);
                foreach (var content in disclaimer.DisclaimerContents)
                {
                    var doc = disclaimerDocs.FirstOrDefault(d => d.GetPropertyValue<int>("DisclaimerId") == disclaimer.DisclaimerId
                                    && d.GetPropertyValue<int?>("LanguageId") == content.LanguageId);
                    var eventSourcing = new DisclaimerCommandEvent()
                    {
                        id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                        EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                        DisclaimerId = disclaimer.DisclaimerId,
                        Name = disclaimer.Name,
                        CreatedBy = disclaimer.CreatedBy,
                        CreatedDate = disclaimer.CreatedDate,
                        UpdatedBy = disclaimer.UpdatedBy,
                        UpdatedDate = disclaimer.UpdatedDate,
                        DefaultCountryId = disclaimer.DefaultCountryId,
                        ProviderName = content.ProviderName,
                        ProviderTerms = content.ProviderTerms,
                        LanguageId = content.LanguageId,
                        DisclaimerContentId = content.DisclaimerContentId,
                        Discriminator = Constants.DisclaimersDiscriminator,
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                foreach(int i in contentToDelete)
                {
                    var deleteEvt = new DisclaimerCommandEvent()
                    {
                        id = disclaimerDocs.FirstOrDefault(d => d.GetPropertyValue<int>("DisclaimerId") == disclaimer.DisclaimerId
                              && d.GetPropertyValue<int>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                        EventType = ServiceBusEventType.Delete,
                        Discriminator = Constants.DisclaimersDiscriminator,
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(deleteEvt);
                }
                scope.Complete();
            }
            return response;
        }
    }
}
