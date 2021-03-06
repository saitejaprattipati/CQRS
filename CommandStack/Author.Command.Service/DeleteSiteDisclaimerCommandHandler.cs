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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class DeleteSiteDisclaimerCommandHandler: IRequestHandler<DeleteSiteDisclaimerCommand, DeleteSiteDisclaimerCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _eventcontext;
        private readonly SiteDisclaimerRepository _siteDisclaimerRepository;
        private readonly CosmosDBContext _context;

        public DeleteSiteDisclaimerCommandHandler(IIntegrationEventPublisherServiceService eventcontext)
        {
            _eventcontext = eventcontext;
            _siteDisclaimerRepository = new SiteDisclaimerRepository(new TaxatHand_StgContext());
            _context = new CosmosDBContext();
        }

        public async Task<DeleteSiteDisclaimerCommandResponse> Handle(DeleteSiteDisclaimerCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteSiteDisclaimerCommandResponse()
            {
                IsSuccessful = false
            };

            var siteDisclaimerIds = request.SiteDisclaimerIds.Distinct().ToList();
            var siteDisclaimers = await _siteDisclaimerRepository.GetSiteDisclaimerByIds(siteDisclaimerIds);
            if (siteDisclaimers.Count != siteDisclaimerIds.Count)
            {
                throw new RulesException("Invalid", @"SiteDisclaimer not found");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
                foreach (var siteDisclaimer in siteDisclaimers)
                {
                    foreach (var sitedisclaimerContent in siteDisclaimer.ArticleContents.ToList())
                    {
                        siteDisclaimer.ArticleContents.Remove(sitedisclaimerContent);
                        _siteDisclaimerRepository.Delete(sitedisclaimerContent);
                    }

                    foreach (var country in siteDisclaimer.ArticleRelatedCountries.ToList())
                    {
                        siteDisclaimer.ArticleRelatedCountries.Remove(country);
                        _siteDisclaimerRepository.Delete(country);
                    }
                    foreach (var countryGroup in siteDisclaimer.ArticleRelatedCountryGroups.ToList())
                    {
                        siteDisclaimer.ArticleRelatedCountryGroups.Remove(countryGroup);
                        _siteDisclaimerRepository.Delete(countryGroup);
                    }
                    foreach (var taxTag in siteDisclaimer.ArticleRelatedTaxTags.ToList())
                    {
                        siteDisclaimer.ArticleRelatedTaxTags.Remove(taxTag);
                        _siteDisclaimerRepository.Delete(taxTag);
                    }

                    foreach (var relatedArticle in siteDisclaimer.RelatedArticlesArticle.ToList())
                    {
                        siteDisclaimer.RelatedArticlesArticle.Remove(relatedArticle);
                        _siteDisclaimerRepository.Delete(relatedArticle);
                    }
                    //Remove reverse relation
                    _siteDisclaimerRepository.DeleteRelatedArticles(siteDisclaimer.ArticleId);

                    foreach (var relatedResource in siteDisclaimer.RelatedResourcesArticle.ToList())
                    {
                        siteDisclaimer.RelatedResourcesArticle.Remove(relatedResource);
                        _siteDisclaimerRepository.Delete(relatedResource);
                    }
                    //Remove reverse relation
                    _siteDisclaimerRepository.DeleteRelatedResources(siteDisclaimer.ArticleId);

                    foreach (var readArticle in siteDisclaimer.UserReadArticles.ToList())
                    {
                        siteDisclaimer.UserReadArticles.Remove(readArticle);
                        _siteDisclaimerRepository.Delete(readArticle);
                    }
                    foreach (var savedArticle in siteDisclaimer.UserSavedArticles.ToList())
                    {
                        siteDisclaimer.UserSavedArticles.Remove(savedArticle);
                        _siteDisclaimerRepository.Delete(savedArticle);
                    }
                    foreach (var contact in siteDisclaimer.ArticleRelatedContacts.ToList())
                    {
                        siteDisclaimer.ArticleRelatedContacts.Remove(contact);
                        _siteDisclaimerRepository.Delete(contact);
                    }

                    _siteDisclaimerRepository.Delete(siteDisclaimer);
                }

                await _siteDisclaimerRepository.UnitOfWork.SaveEntitiesAsync();

                response.IsSuccessful = true;
                scope.Complete();
            }

            using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var disclaimerDocs = _context.GetAll(Constants.ArticlesDiscriminator);
                foreach (var sitedisclaimer in siteDisclaimers)
                {
                    foreach (var doc in disclaimerDocs.Where(d => d.GetPropertyValue<int>("ArticleId") == sitedisclaimer.ArticleId))
                    {
                        var eventSource = new ArticleCommandEvent
                        {
                            id = doc.GetPropertyValue<Guid>("id"),
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.ArticlesDiscriminator,
                            PartitionKey = doc.GetPropertyValue<int>("LanguageId").ToString()
                        };
                        await _eventcontext.PublishThroughEventBusAsync(eventSource);
                    }
                }
                scope.Complete();
            }
            return response;
        }
    }
}
