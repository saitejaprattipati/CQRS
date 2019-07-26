using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework.ExceptionHandling;
using AutoMapper;
using MediatR;
using System;
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

        public UpdateContentDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext, IMapper mapper)
        {
            _eventcontext = eventcontext;
            _contentDisclaimerRepository = new ContentDisclaimerRepository(new TaxatHand_StgContext());
            _mapper = mapper;
        }

        public async Task<UpdateContentDisclaimerCommandResponse> Handle(UpdateContentDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateContentDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //Update existing Content disclaimer
                var disclaimer = await _contentDisclaimerRepository.GetContentDisclaimer(request.DisclaimerId);

                if (disclaimer == null)
                {
                    throw new RulesException("contentDisclaimer", $"ContentDisclaimer with DisclaimerId: {request.DisclaimerId}  not found");
                }

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
                    if (request.DisclaimerContent.Where(dc=>dc.LanguageId.Equals(item.LanguageId)).Count() == 0)
                    {
                        disclaimer.DisclaimerContents.Remove(item);
                        _contentDisclaimerRepository.Delete(item);
                    }
                }

                await _contentDisclaimerRepository.UnitOfWork.SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            return response;
        }
    }
}
