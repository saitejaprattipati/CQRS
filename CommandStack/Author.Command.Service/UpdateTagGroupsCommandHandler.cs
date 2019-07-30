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
using System.Transactions;

namespace Author.Command.Service
{
   public class UpdateTagGroupsCommandHandler : IRequestHandler<UpdateTagGroupsCommand, UpdateTagGroupsCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly TagGroupsRepository _taxTagsRepository;
        private readonly ILogger _logger;

        public UpdateTagGroupsCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<CreateTagGroupsCommandHandler> logger)
        {
            _taxTagsRepository = new TagGroupsRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
        }
        public async Task<UpdateTagGroupsCommandResponse> Handle(UpdateTagGroupsCommand request, CancellationToken cancellationToken)
        {
            UpdateTagGroupsCommandResponse response = new UpdateTagGroupsCommandResponse()
            {
                IsSuccessful = false
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<int> objTagGroups = new List<int>();
                objTagGroups.Add(request.TagGroupsId);
                var taxGroup = _taxTagsRepository.GetTagGroups(objTagGroups)[0];
                List<Languages> languages = _taxTagsRepository.GetAllLanguages();

                foreach (var content in request.LanguageName)
                {
                    var taxGroupContents = taxGroup.TaxTagContents.Where(s => s.LanguageId == content.LanguageId).FirstOrDefault();
                    if (taxGroupContents == null)
                    {
                        TaxTagContents objtaxGroupContents = new TaxTagContents();
                        objtaxGroupContents.DisplayName = content.Name;
                        objtaxGroupContents.LanguageId = content.LanguageId;
                        taxGroup.TaxTagContents.Add(objtaxGroupContents);
                    }
                    else
                    {
                        taxGroupContents.DisplayName = content.Name;
                        _taxTagsRepository.Update(taxGroupContents);
                    }
                }
                //  List<TaxTagContents> ResourceGroupContents = taxGroup.TaxTagContents.Where(s => s.TaxTagId == request.TagGroupsId).ToList();
                foreach (var resourceContent in taxGroup.TaxTagContents.ToList())
                {
                    if (request.LanguageName.Where(s => s.LanguageId == resourceContent.LanguageId).Count() == 0)
                    {
                        taxGroup.TaxTagContents.Remove(resourceContent);
                        _taxTagsRepository.Delete(resourceContent);
                    }
                }
                taxGroup.UpdatedBy = "";
                taxGroup.UpdatedDate = DateTime.Now;
                await _taxTagsRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
