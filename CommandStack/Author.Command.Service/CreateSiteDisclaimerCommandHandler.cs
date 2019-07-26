using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class CreateSiteDisclaimerCommandHandler : IRequestHandler<CreateSiteDisclaimerCommand, CreateSiteDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SiteDisclaimerRepository _siteDisclaimerRepository;

        public CreateSiteDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _siteDisclaimerRepository = new SiteDisclaimerRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
        }

        public async Task<CreateSiteDisclaimerCommandResponse> Handle(CreateSiteDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateSiteDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var siteDisclaimer = new Articles
                {
                    PublishedDate = DateTime.ParseExact(request.PublishedDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToUniversalTime(),
                    Author = request.Author,
                    Type = Convert.ToInt32(ArticleType.Page),
                    SubType = request.ArticleType,
                    IsPublished = true,
                    CreatedBy = "CMS Admin",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                foreach (var item in request.LanguageContent)
                {
                    var siteDisclaimerContent = new ArticleContents
                    {
                        LanguageId = item.LanguageId,
                        Title = item.Title,
                        TeaserText = item.TeaserText,
                        Content = item.Body
                    };
                    siteDisclaimer.ArticleContents.Add(siteDisclaimerContent);
                }

                await _siteDisclaimerRepository.AddAsync(siteDisclaimer);

                await _siteDisclaimerRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            return response;
        }
    }
}
