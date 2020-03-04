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

            Disclaimers DisclaimersDetails = new Disclaimers();
            ResourceGroups ResourceGroupsDetails = new ResourceGroups();
            Provinces ProvincesDetails = new Provinces();
            List<TaxTags> taxTagsDetails = new List<TaxTags>();
            List<Articles> articlesDetails = new List<Articles>();
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

            if (request.Operation == "Publish" || request.Operation == "UnPublish")
            {
                foreach (var article in articles)
                {
                    taxTagsDetails = _ArticleRepository.getTaxTagsDetailsByIds(article.ArticleRelatedTaxTags.Select(s => s.TaxTagId).ToList());
                    articlesDetails = _ArticleRepository.getArticleCompleteDataById(article.RelatedArticlesArticle.Select(s => s.RelatedArticleId).ToList());
                    ResourceGroupsDetails = article.ResourceGroupId == null ? null : _ArticleRepository.getResourceGroupById(int.Parse(article.ResourceGroupId.ToString()));
                    ProvincesDetails = article.ProvinceId == null ? null : _ArticleRepository.getProvisionsById(int.Parse(article.ProvinceId.ToString()));
                    DisclaimersDetails = article.DisclaimerId == null ? null : _ArticleRepository.getDisclaimerById(int.Parse(article.DisclaimerId.ToString()));
                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var doc in articleDocs.Where(d => d.GetPropertyValue<int>("ArticleId") == article.ArticleId))
                        {


                            var DisclaimerLanguageId = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37;
                            var ResourceGroupLanguageId = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37;
                            var ProvisionsLanguageId = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37;


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
                                Disclaimer = new DisclamersSchema { DisclaimerId = int.Parse(article.DisclaimerId.ToString()), ProviderName = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == DisclaimerLanguageId).Select(ds => ds.ProviderName).FirstOrDefault(), ProviderTerms = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == DisclaimerLanguageId).Select(ds => ds.ProviderTerms).FirstOrDefault() },
                                ResourceGroup = new ResourceGroupsSchema { ResourceGroupId = int.Parse(article.ResourceGroupId.ToString()), GroupName = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == ResourceGroupLanguageId).Select(ds => ds.GroupName).FirstOrDefault(), Position = ResourceGroupsDetails.Position },
                                IsPublished = request.Operation == "Publish" ? true : false,
                                CreatedDate = article.CreatedDate,
                                CreatedBy = article.CreatedBy,
                                UpdatedDate = article.UpdatedDate,
                                UpdatedBy = article.UpdatedBy,
                                NotificationSentDate = article.NotificationSentDate,
                                Provisions = new ProvisionsSchema { ProvinceId = int.Parse(article.ProvinceId.ToString()), DisplayName = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == ProvisionsLanguageId).Select(ds => ds.DisplayName).FirstOrDefault() },
                                ArticleContentId = doc.GetPropertyValue<int>("ArticleContentId"),
                                LanguageId = doc.GetPropertyValue<int>("LanguageId"),
                                Title = doc.GetPropertyValue<string>("Title"),
                                TeaserText = doc.GetPropertyValue<string>("TeaserText"),
                                Content = doc.GetPropertyValue<string>("Content"),
                                RelatedContacts = article.ArticleRelatedContacts.Select(s => new RelatedEntityId { IdVal = s.ContactId }).ToList(),
                                RelatedCountries = article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList(),
                                RelatedCountryGroups = article.ArticleRelatedCountryGroups.Select(s => new RelatedEntityId { IdVal = s.CountryGroupId }).ToList(),
                                RelatedTaxTags = article.ArticleRelatedTaxTags.Select(s => { var RelatedtaxTagLanguageId = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37; return new RelatedTaxTagsSchema { TaxTagId = s.TaxTagId, DisplayName = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == RelatedtaxTagLanguageId).Select(ttcs => ttcs.DisplayName).FirstOrDefault() }; }).ToList(),
                                RelatedArticles = article.RelatedArticlesArticle.Select(s => { var RelatedArticleLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedArticleLanguageId).Select(v => v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                                RelatedResources = article.RelatedResourcesArticle.Select(s => { var RelatedResourceLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == doc.GetPropertyValue<int>("LanguageId")).Count() > 0 ? doc.GetPropertyValue<int>("LanguageId") : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedResourceLanguageId).Select(v => v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                                Discriminator = Constants.ArticlesDiscriminator,
                                PartitionKey = ""
                            };
                            await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                        }
                        scope.Complete();
                    }
                }
            }
            else if (request.Operation == "Delete")
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
                    scope.Complete();
                }
            }

            return response;
        }
    }
}
