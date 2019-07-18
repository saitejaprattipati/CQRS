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
    public class ManipulateTaxGroupCommandHandler : IRequestHandler<ManipulateTaxGroupCommand, ManipulateTaxGroupCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly TagGroupsRepository _tagGroupRepository;
        private readonly ILogger _logger;
        public ManipulateTaxGroupCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateTaxGroupCommandHandler> logger)
        {
            _tagGroupRepository = new TagGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }

        public async Task<ManipulateTaxGroupCommandResponse> Handle(ManipulateTaxGroupCommand request, CancellationToken cancellationToken)
        {
            ManipulateTaxGroupCommandResponse response = new ManipulateTaxGroupCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<TaxTags> tagGroups = _tagGroupRepository.GetTagGroups(request.TaxGroupIds);
                if (request.TaxGroupIds.Count != tagGroups.Count)
                    throw new RulesException("Invalid", @"TagGroup not found");

                if (request.Operation == "Publish")
                {
                    foreach (var taggroup in tagGroups)
                    {
                        taggroup.IsPublished = true;
                        _tagGroupRepository.Update<TaxTags>(taggroup);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var taggroup in tagGroups)
                    {
                        taggroup.IsPublished = false;
                        _tagGroupRepository.Update<TaxTags>(taggroup);
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (TaxTags taggroup in tagGroups)
                    {
                        foreach (var tagGroupContents in taggroup.TaxTagContents.ToList())
                        {
                            taggroup.TaxTagContents.Remove(tagGroupContents);
                            _tagGroupRepository.Delete<TaxTagContents>(tagGroupContents);
                        }
                        _tagGroupRepository.DeletetagGroup(taggroup);
                    }
                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _tagGroupRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
