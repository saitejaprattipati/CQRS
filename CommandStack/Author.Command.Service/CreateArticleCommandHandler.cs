//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Author.Command.Service
{
    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string LastName { get; set; }
    }
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CreateArticleCommandResponse>
    {
        // private readonly IArticleRepository _context;
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ArticleRepository _ArticleRepository;
        //  private readonly ILogger _logger;

        public CreateArticleCommandHandler(IIntegrationEventPublisherServiceService Eventcontext)
        {
            _ArticleRepository = new ArticleRepository(new TaxatHand_StgContext());
            // _context = new ArticleRepository();
            _Eventcontext = Eventcontext;
            //    _logger = logger;
        }
        public async Task<CreateArticleCommandResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            CreateArticleCommandResponse response = new CreateArticleCommandResponse()
            {
                IsSuccessful = false
            };
            try
            {
                List<Articles> articles = _ArticleRepository.getArticles();
                List<TaxTags> taxTags = _ArticleRepository.getTaxTags();
                List<Countries> countries = _ArticleRepository.getCountries();
                List<CountryGroups> countryGroups = _ArticleRepository.getCountryGroups();
                List<Contacts> contacts = _ArticleRepository.getContacts();
                Articles _article = new Articles();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _article.Type = request.Type;
                    _article.Author = request.Author;
                    _article.PublishedDate = request.PublishedDate.Value;
                    _article.SubType = request.SubType;
                    _article.ResourceGroupId = request.ResourceGroupId > 0 ? request.ResourceGroupId : (int?)null;
                    _article.ResourcePosition = request.ResourcePosition > 0 ? request.ResourcePosition : (int?)null;
                    _article.IsPublished = request.IsPublished;
                    _article.CreatedDate = DateTime.UtcNow;
                    _article.UpdatedDate = DateTime.UtcNow;
                    _article.ArticleContents = new List<ArticleContents>();
                    _article.CreatedBy = request.UpdatedBy;
                    _article.UpdatedBy = request.UpdatedBy;
                    _article.DisclaimerId = request.DisclaimerId;
                    _article.ImageId = request.ImageId;
                    _article.State = request.State;
                    _article.ProvinceId = request.ProvinceId;
                    foreach (var acontent in request.ArticleContent)
                    {
                        ArticleContents ac = new ArticleContents()
                        {
                            LanguageId = acontent.LanguageId,
                            Content = acontent.Content,
                            TeaserText = acontent.TeaserText,
                            Title = acontent.Title
                        };
                        _article.ArticleContents.Add(ac);
                    }
                    if (request.RelatedArticles != null)
                    {
                        var rArticleList = request.RelatedArticles;
                        var relatedArticles = articles.Where(a => rArticleList.Any(ra => ra == a.ArticleId)).ToList();
                        foreach (Articles article in relatedArticles)
                        {
                            _article.RelatedArticlesArticle.Add(new RelatedArticles()
                            {
                                RelatedArticleId = article.ArticleId,
                                Article = article
                            });
                        };
                        //Add reverse relation
                        //relatedArticles.ForEach(a => a.RelatedArticlesArticle.Add(new RelatedArticles {Article = Article }));
                    }
                    if (request.RelatedResources != null)
                    {
                        var rResourceList = request.RelatedResources.ToList();
                        //var relatedResources = context.Articles.Where(a => rResourceList.Any(ra => ra == a.ArticleId)).ToList();
                        var relatedResources = articles.Where(a => rResourceList.Contains(a.ArticleId)).ToList();
                        foreach (Articles article in relatedResources)
                        {
                            _article.RelatedResourcesArticle.Add(new RelatedResources()
                            {
                                RelatedArticleId = article.ArticleId,
                                Article = article
                            });
                        };
                    }
                    if (request.RelatedTaxTags != null)
                    {
                        var rTagList = request.RelatedTaxTags.ToList();
                        //var relatedTags = context.TaxTags.Where(t => rTagList.Any(tt => tt == t.TaxTagId.ToString())).ToList();
                        var relatedTags = taxTags.Where(t => rTagList.Contains(t.TaxTagId)).ToList();
                        //Article.ArticleRelatedTaxTags = relatedTags;

                        foreach (TaxTags tags in relatedTags)
                        {
                            _article.ArticleRelatedTaxTags.Add(new ArticleRelatedTaxTags()
                            {
                                TaxTagId = tags.TaxTagId,
                                TaxTag = tags
                            });
                        };
                    }
                    if (request.RelatedCountries != null)
                    {
                        var rCountries = request.RelatedCountries.ToList();
                        //var relatedCountries = context.Countries.Where(c => rCountries.Any(rc => rc == c.CountryId.ToString())).ToList();
                        var relatedCountries = countries.Where(c => rCountries.Contains(c.CountryId)).ToList();
                        //articleobject.ArticleRelatedCountries = relatedCountries;
                        foreach (Countries country in relatedCountries)
                        {
                            _article.ArticleRelatedCountries.Add(new ArticleRelatedCountries()
                            {
                                CountryId = country.CountryId,
                                Country = country
                            });
                        };
                    }
                    if (request.RelatedCountryGroups != null)
                    {
                        var rCountryGroups = request.RelatedCountryGroups.ToList();
                        //var relatedCountryGroups = context.CountryGroups.Where(c => rCountryGroups.Any(rc => rc == c.CountryGroupId)).ToList();
                        var relatedCountryGroups = countryGroups.Where(c => rCountryGroups.Contains(c.CountryGroupId)).ToList();
                        // articleobject.ArticleRelatedCountryGroups = relatedCountryGroups;
                        foreach (CountryGroups cGroup in relatedCountryGroups)
                        {
                            _article.ArticleRelatedCountryGroups.Add(new ArticleRelatedCountryGroups()
                            {
                                CountryGroupId = cGroup.CountryGroupId,
                                CountryGroup = cGroup
                            });
                        };
                    }
                    if (request.RelatedContacts != null)
                    {
                        var rContacts = request.RelatedContacts.ToList();
                        //var relatedContacts = context.Contacts.Where(c => rContacts.Any(rc => rc == c.ContactId)).ToList();
                        var relatedContacts = contacts.Where(c => rContacts.Contains(c.ContactId)).ToList();
                        // articleobject.ArticleRelatedContacts = relatedContacts;
                        foreach (Contacts rcontact in relatedContacts)
                        {
                            _article.ArticleRelatedContacts.Add(new ArticleRelatedContacts()
                            {
                                ContactId = rcontact.ContactId,
                                Contact = rcontact
                            });
                        };
                    }
                    //Push logic needs to be implemented
                    int userCount = _ArticleRepository.SendNotificationsForArticle(request);
                    if (userCount > 0) { _article.NotificationSentDate = DateTime.Now; }
                    _ArticleRepository.Add(_article);
                    await _ArticleRepository.UnitOfWork
                       .SaveEntitiesAsync();

                    response.IsSuccessful = true;
                    scope.Complete();
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var content in _article.ArticleContents)
                    {
                        var eventSourcing = new ArticleCommandEvent()
                        {
                            EventType = ServiceBusEventType.Create,
                            ArticleId = _article.ArticleId,
                            PublishedDate = _article.PublishedDate,
                            Author = _article.Author,
                            ImageId = _article.ImageId,
                            State = _article.State,
                            Type = _article.Type,
                            SubType = _article.SubType,
                            ResourcePosition = _article.ResourcePosition,
                            DisclaimerId = _article.DisclaimerId,
                            ResourceGroupId = _article.ResourceGroupId,
                            IsPublished = _article.IsPublished,
                            CreatedDate = _article.CreatedDate,
                            CreatedBy = _article.CreatedBy,
                            UpdatedDate = _article.UpdatedDate,
                            UpdatedBy = _article.UpdatedBy,
                            NotificationSentDate = _article.NotificationSentDate,
                            ProvinceId = _article.ProvinceId,
                            ArticleContentId = content.ArticleContentId,
                            LanguageId = content.LanguageId,
                            Title = content.Title,
                            TeaserText = content.TeaserText,
                            Content = content.Content,
                            RelatedContacts = _article.ArticleRelatedContacts.Select(s => new RelatedEntityId { IdVal = s.ContactId }).ToList(),
                            RelatedCountries = _article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList(),
                            RelatedCountryGroups = _article.ArticleRelatedCountryGroups.Select(s => new RelatedEntityId { IdVal = s.CountryGroupId }).ToList(),
                            RelatedTaxTags = _article.ArticleRelatedTaxTags.Select(s => new RelatedEntityId { IdVal = s.TaxTagId }).ToList(),
                            RelatedArticles = _article.RelatedArticlesArticle.Select(s => new RelatedEntityId { IdVal = s.ArticleId }).ToList(),
                            RelatedResources = _article.RelatedResourcesArticle.Select(s => new RelatedEntityId { IdVal = s.RelatedArticleId }).ToList(),
                            Discriminator = Constants.ArticlesDiscriminator
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                    }
                    scope.Complete();
                }
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.FailureReason = "Technical Error";
                // _logger.LogError(ex, "Error while handling command");
            }
            return response;
        }
    }
}
