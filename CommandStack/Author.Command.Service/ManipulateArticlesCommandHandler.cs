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
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;
using Author.Command.Domain.Models;

namespace Author.Command.Service
{
    public class ManipulateArticlesCommandHandler : IRequestHandler<ManipulateArticlesCommand, ManipulateArticlesCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ArticleRepository _ArticleRepository;
        private readonly ILogger _logger;
        private readonly CosmosDBContext _context;

        public ManipulateArticlesCommandHandler(IIntegrationEventPublisherServiceService Eventcontext, ILogger<ManipulateArticlesCommandHandler> logger)
        {
            _ArticleRepository = new ArticleRepository(new TaxatHand_StgContext());
            _Eventcontext = Eventcontext;
            _logger = logger;
            _context = new CosmosDBContext();
        }
        public async Task<ManipulateArticlesCommandResponse> Handle(ManipulateArticlesCommand request, CancellationToken cancellationToken)
        {
            ManipulateArticlesCommandResponse response = new ManipulateArticlesCommandResponse()
            {
                IsSuccessful = false
            };
            List<Articles> articles = _ArticleRepository.getArticleCompleteDataById(request.ArticlesIds);
            if (request.ArticlesIds.Count != articles.Count)
                throw new RulesException("Invalid", @"Country not found");
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish")
                {
                    foreach (var article in articles)
                    {
                        article.IsPublished = true;
                        _ArticleRepository.Update<Articles>(article);
                    }
                }
                else if (request.Operation == "UnPublish")
                {
                    foreach (var article in articles)
                    {
                        article.IsPublished = false;
                        _ArticleRepository.Update<Articles>(article);
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (Articles article in articles)
                    {
                        foreach (var content in article.ArticleContents.ToList())
                        {
                            article.ArticleContents.Remove(content);
                            _ArticleRepository.Delete<ArticleContents>(content);
                        }
                        foreach (var country in article.ArticleRelatedCountries.ToList())
                        {
                            article.ArticleRelatedCountries.Remove(country);
                        }
                        foreach (var countryGroup in article.ArticleRelatedCountryGroups.ToList())
                        {
                            article.ArticleRelatedCountryGroups.Remove(countryGroup);
                        }
                        foreach (var taxTag in article.ArticleRelatedTaxTags.ToList())
                        {
                            article.ArticleRelatedTaxTags.Remove(taxTag);
                        }
                        foreach (var relatedArticle in article.RelatedArticlesArticle.ToList())
                        {
                            article.RelatedArticlesArticle.Remove(relatedArticle);
                            //Remove reverse relation
                            // relatedArticle.RelatedArticles.Remove(article);
                        }
                        foreach (var relatedResource in article.RelatedResourcesArticle.ToList())
                        {
                            article.RelatedResourcesArticle.Remove(relatedResource);
                        }
                        foreach (var readArticle in article.UserReadArticles.ToList())
                        {
                            article.UserReadArticles.Remove(readArticle);
                            _ArticleRepository.Delete<UserReadArticles>(readArticle);
                        }
                        foreach (var savedArticle in article.UserSavedArticles.ToList())
                        {
                            article.UserSavedArticles.Remove(savedArticle);
                            _ArticleRepository.Delete<UserSavedArticles>(savedArticle);
                        }
                        foreach (var contact in article.ArticleRelatedContacts.ToList())
                        {
                            article.ArticleRelatedContacts.Remove(contact);
                        }
                        _ArticleRepository.DeleteArticle(article);
                    }
                }
                else
                    throw new RulesException("Operation", @"The Operation " + request.Operation + " is not valied");
                await _ArticleRepository.UnitOfWork
                   .SaveEntitiesAsync();
                response.IsSuccessful = true;
                scope.Complete();
            }
            var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Operation == "Publish" || request.Operation == "UnPublish")
                {
                    foreach (var article in articles)
                    {
                        foreach (var doc in articleDocs.Where(d => d.GetPropertyValue<int>("ArticleId") == article.ArticleId))
                        {
                            var eventSourcing = new ArticleCommandEvent()
                            {
                                id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                                EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                                ArticleId = article.ArticleId,
                                PublishedDate = article.PublishedDate,
                                Author = article.Author,
                                ImageId = article.ImageId,
                                State = article.State,
                                Type = article.Type,
                                SubType = article.SubType,
                                ResourcePosition = article.ResourcePosition,
                                DisclaimerId = article.DisclaimerId,
                                ResourceGroupId = article.ResourceGroupId,
                                IsPublished = request.Operation=="Publish"?true:false,
                                CreatedDate = article.CreatedDate,
                                CreatedBy = article.CreatedBy,
                                UpdatedDate = article.UpdatedDate,
                                UpdatedBy = article.UpdatedBy,
                                NotificationSentDate = article.NotificationSentDate,
                                ProvinceId = article.ProvinceId,
                                ArticleContentId = doc.GetPropertyValue<int>("ArticleContentId"),
                                LanguageId = doc.GetPropertyValue<int>("LanguageId"),
                                Title = doc.GetPropertyValue<string>("Title"),
                                TeaserText = doc.GetPropertyValue<string>("TeaserText"),
                                Content = doc.GetPropertyValue<string>("Content"),
                                RelatedContacts = article.ArticleRelatedContacts.Select(s => new RelatedEntityId { IdVal = s.ContactId }).ToList(),
                                RelatedCountries = article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList(),
                                RelatedCountryGroups = article.ArticleRelatedCountryGroups.Select(s => new RelatedEntityId { IdVal = s.CountryGroupId }).ToList(),
                                RelatedTaxTags = article.ArticleRelatedTaxTags.Select(s => new RelatedEntityId { IdVal = s.TaxTagId }).ToList(),
                                RelatedArticles = article.RelatedArticlesArticle.Select(s => new RelatedEntityId { IdVal = s.RelatedArticleId }).ToList(),
                                RelatedResources = article.RelatedResourcesArticle.Select(s => new RelatedEntityId { IdVal = s.RelatedArticleId }).ToList(),
                                Discriminator = Constants.ArticlesDiscriminator,
                                PartitionKey = ""
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                        }
                    }
                }
                else if (request.Operation == "Delete")
                {
                    foreach (var item in articles)
                    {
                        foreach (var doc in articleDocs.Where(d => d.GetPropertyValue<int>("ArticleId") == item.ArticleId))
                        {
                            var articleevent = new ArticleCommandEvent()
                            {
                                id = doc.GetPropertyValue<Guid>("id"),
                                EventType = ServiceBusEventType.Delete,
                                Discriminator = Constants.ArticlesDiscriminator,
                                PartitionKey = doc.GetPropertyValue<int>("LanguageId").ToString()
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(articleevent);
                        }
                    }
                }
                scope.Complete();
            }
            return response;
        }
    }
}
