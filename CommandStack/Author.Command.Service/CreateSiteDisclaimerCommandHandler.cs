using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using MediatR;
using System;
using System.Globalization;
using System.Linq;
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
            Disclaimers DisclaimersDetails = new Disclaimers();
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

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
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
                DisclaimersDetails = siteDisclaimer.DisclaimerId == null ? null : _siteDisclaimerRepository.getDisclaimerById(int.Parse(siteDisclaimer.DisclaimerId.ToString()));
                response.IsSuccessful = true;
                scope.Complete();
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var content in siteDisclaimer.ArticleContents)
                {
                    var eventSourcing = new ArticleCommandEvent()
                    {
                        EventType = ServiceBusEventType.Create,
                        Discriminator = Constants.ArticlesDiscriminator,
                        ArticleId = siteDisclaimer.ArticleId,
                        CreatedBy = siteDisclaimer.CreatedBy ?? string.Empty,
                        CreatedDate = siteDisclaimer.CreatedDate.ToString(),
                        UpdatedBy = siteDisclaimer.UpdatedBy ?? string.Empty,
                        UpdatedDate = siteDisclaimer.UpdatedDate.ToString(),
                        PublishedDate = siteDisclaimer.PublishedDate.ToString(),
                        IsPublished = siteDisclaimer.IsPublished,
                        Author = siteDisclaimer.Author ?? string.Empty,
                        Type = siteDisclaimer.Type,
                        SubType = siteDisclaimer.SubType,
                        LanguageId = content.LanguageId,
                        Title = content.Title,
                        TeaserText = content.TeaserText,
                        Content = content.Content,
                        ArticleContentId = content.ArticleContentId,
                        Disclaimer = new DisclamersSchema { DisclaimerId = int.Parse(siteDisclaimer.DisclaimerId.ToString()), ProviderName = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == content.LanguageId).Select(ds => ds.ProviderName).FirstOrDefault(), ProviderTerms = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == content.LanguageId).Select(ds => ds.ProviderTerms).FirstOrDefault() },                      
                        PartitionKey = ""
                    };
                    await _eventcontext.PublishThroughEventBusAsync(eventSourcing);
                }
                scope.Complete();
            }

            return response;
        }
    }
}
