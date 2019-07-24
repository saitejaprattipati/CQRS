using Author.Command.Domain.Command;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Framework.ExceptionHandling;
using MediatR;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class UpdateSiteDisclaimerCommandHandler : IRequestHandler<UpdateSiteDisclaimerCommand, UpdateSiteDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SiteDisclaimerRepository _siteDisclaimerRepository;
        private readonly LanguageRepository _languageRepository;


        public UpdateSiteDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _siteDisclaimerRepository = new SiteDisclaimerRepository(new TaxatHand_StgContext());
            _languageRepository = new LanguageRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
        }

        public async Task<UpdateSiteDisclaimerCommandResponse> Handle(UpdateSiteDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateSiteDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //Update existing disclaimer
                var siteDisclaimer = await _siteDisclaimerRepository.GetSiteDisclaimer(request.SiteDisclaimerId);

                if (siteDisclaimer == null)
                {
                    throw new RulesException("siteDisclaimer",$"SiteDisclaimer with SiteDisclaimerId: {request.SiteDisclaimerId}  not found");
                }

                siteDisclaimer.Type = Convert.ToInt32(ArticleType.Page);
                siteDisclaimer.SubType = request.ArticleType;
                siteDisclaimer.Author = request.Author;
                siteDisclaimer.PublishedDate = DateTime.ParseExact(request.PublishedDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToUniversalTime();

                foreach (var item in request.LanguageContent)
                {
                    var siteDisclaimerContent = siteDisclaimer.ArticleContents.FirstOrDefault(x => x.LanguageId.Equals(item.LanguageId));
                    if (siteDisclaimerContent == null)
                    {
                        siteDisclaimerContent = new ArticleContents { LanguageId = item.LanguageId, Content = item.Body, TeaserText = item.TeaserText, Title = item.Title };
                        siteDisclaimer.ArticleContents.Add(siteDisclaimerContent);
                    }
                    else
                    {
                        siteDisclaimerContent.Content = item.Body;
                        siteDisclaimerContent.TeaserText = item.TeaserText;
                        siteDisclaimerContent.Title = item.Title;
                        _siteDisclaimerRepository.Update(siteDisclaimerContent);
                    }
                }

                foreach (var item in siteDisclaimer.ArticleContents.ToList())
                {
                    if (request.LanguageContent.Where(s => s.LanguageId == item.LanguageId).Count() == 0)
                    {
                        siteDisclaimer.ArticleContents.Remove(item);
                        _siteDisclaimerRepository.Delete(item);
                    }
                }

                siteDisclaimer.UpdatedBy = "CMS Admin";
                siteDisclaimer.UpdatedDate = DateTime.UtcNow;
                await _siteDisclaimerRepository.UnitOfWork.SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }

            return response;
        }
    }
}
