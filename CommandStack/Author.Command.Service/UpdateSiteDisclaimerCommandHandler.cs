using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Framework.ExceptionHandling;
using Author.Core.Services.Persistence.CosmosDB;
using MediatR;
using System;
using System.Collections.Generic;
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
        private readonly CosmosDBContext _context;

        public UpdateSiteDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _siteDisclaimerRepository = new SiteDisclaimerRepository(new TaxatHand_StgContext());
            _eventcontext = eventcontext;
            _context = new CosmosDBContext();
        }

        public async Task<UpdateSiteDisclaimerCommandResponse> Handle(UpdateSiteDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateSiteDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            var siteDisclaimer = await _siteDisclaimerRepository.GetSiteDisclaimer(request.SiteDisclaimerId);

            if (siteDisclaimer == null)
            {
                throw new RulesException("siteDisclaimer", $"SiteDisclaimer with SiteDisclaimerId: {request.SiteDisclaimerId}  not found");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //Update existing disclaimer
                
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
                        siteDisclaimerContent.LanguageId = item.LanguageId;
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

            using(TransactionScope scope =new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var disclaimerdocs = _context.GetAll(Constants.ArticlesDiscriminator).Records as IEnumerable<ArticleCommandEvent>;
                foreach(var item in siteDisclaimer.ArticleContents)
                {
                    foreach(var doc in disclaimerdocs.Where(d => d.ArticleID == item.ArticleId && d.LanguageId == item.LanguageId))
                    {
                        var eventSource = new ArticleCommandEvent
                        {
                            id = doc.id,
                            EventType = ServiceBusEventType.Update,
                            Discriminator = Constants.ArticlesDiscriminator,
                            Type = siteDisclaimer.Type,
                            SubType = siteDisclaimer.SubType,
                            Author = siteDisclaimer.Author,
                            PublishedDate = siteDisclaimer.PublishedDate,
                            Title = item.Title,
                            TeaserText = item.TeaserText,
                            Content = item.Content,
                            LanguageId = item.LanguageId,
                            UpdatedBy = siteDisclaimer.UpdatedBy,
                            UpdatedDate = siteDisclaimer.UpdatedDate
                        };
                        await _eventcontext.PublishThroughEventBusAsync(eventSource);
                    }
                }
            }

            return response;
        }
    }
}
