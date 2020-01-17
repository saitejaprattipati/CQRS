﻿using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Framework.ExceptionHandling;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class DeleteContentDisclaimerCommandHandler : IRequestHandler<DeleteContentDisclaimerCommand, DeleteContentDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly ContentDisclaimerRepository _contentDisclaimerRepository;

        public DeleteContentDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _eventcontext = eventcontext;
            _contentDisclaimerRepository = new ContentDisclaimerRepository(new TaxatHand_StgContext());
        }

        public async Task<DeleteContentDisclaimerCommandResponse> Handle(DeleteContentDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteContentDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            var disclaimerIds = request.DisclaimerIds.Distinct().ToList();
            var disclaimers = await _contentDisclaimerRepository.GetDisclaimerByIds(disclaimerIds);
            if (disclaimers.Count != disclaimerIds.Count)
            {
                throw new RulesException("Invalid", @"ContentDisclaimer not found");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
                foreach (var disclaimer in disclaimers)
                {
                    foreach (var disclaimerContent in disclaimer.DisclaimerContents.ToList())
                    {
                        disclaimer.DisclaimerContents.Remove(disclaimerContent);
                        _contentDisclaimerRepository.Delete(disclaimerContent);
                    }
                    _contentDisclaimerRepository.Delete(disclaimer);
                }

                await _contentDisclaimerRepository.UnitOfWork.SaveEntitiesAsync();

                response.IsSuccessful = true;
                scope.Complete();
            }
            foreach(var content in disclaimers)
            {
                var eventSourcing = new DisclaimerCommandEvent()
                {
                    EventType = ServiceBusEventType.Delete,
                    Discriminator = Constants.DisclaimersDiscriminator,
                    DisclaimerId = content.DisclaimerId
                };
                await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
            }
            return response;
        }
    }
}
