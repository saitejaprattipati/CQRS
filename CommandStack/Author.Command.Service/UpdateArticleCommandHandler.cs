using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using Author.Core.Framework;
using Author.Core.Services.Persistence.CosmosDB;
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
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, UpdateArticleCommandResponse>
    {
        private readonly IIntegrationEventPublisherServiceService _Eventcontext;
        private readonly ArticleRepository _ArticleRepository;
        private readonly CosmosDBContext _context;
        //  private readonly ILogger _logger;

        public UpdateArticleCommandHandler(IIntegrationEventPublisherServiceService Eventcontext)
        {
            _ArticleRepository = new ArticleRepository(new TaxatHand_StgContext());
            // _context = new ArticleRepository();
            _Eventcontext = Eventcontext;
            _context = new CosmosDBContext();
            //    _logger = logger;
        }
        public async Task<UpdateArticleCommandResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            UpdateArticleCommandResponse response = new UpdateArticleCommandResponse()
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
                Articles _article = _ArticleRepository.getArticleCompleteDataById(new List<int> { request.ArticleID } )[0];
                var contentToDelete = new List<int>();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _article.UpdatedBy = request.UpdatedBy;
                    _article.UpdatedDate = DateTime.UtcNow;
                    _article.Author = request.Author;
                    _article.DisclaimerId = request.DisclaimerId;
                    _article.ResourceGroupId = request.ResourceGroupId > 0 ? request.ResourceGroupId : (int?)null;
                    _article.ResourcePosition = request.ResourcePosition > 0 ? request.ResourcePosition : (int?)null;

                    if (request.PublishedDate != null) _article.PublishedDate = request.PublishedDate.Value;
                    _article.SubType = request.SubType;
                    _article.State = request.State;
                    _article.ProvinceId = request.ProvinceId;
                    _article.IsPublished = request.IsPublished;//= Action == "Publish";
                    //update article content
                    //delete removed languages, update existing, add new languages
                    foreach (var content in request.ArticleContent)
                    {
                        var artContent =
                            _article.ArticleContents.FirstOrDefault(v => v.LanguageId == content.LanguageId);
                        if (artContent == null)
                        {
                            var newContent = new ArticleContents
                            {
                                Content = content.Content,
                                LanguageId = content.LanguageId,
                                TeaserText = content.TeaserText,
                                Title = content.Title
                            };

                            //content.ArticleId = articleobject.ArticleId;
                            _article.ArticleContents.Add(newContent);
                        }
                        else
                        {
                            artContent.Content = content.Content;
                            artContent.LanguageId = content.LanguageId;
                            artContent.TeaserText = content.TeaserText;
                            artContent.Title = content.Title;
                            _ArticleRepository.Update<ArticleContents>(artContent);
                        }
                    }
                    var articleContentIds = _article.ArticleContents.Select(c => c.LanguageId).ToList();
                    var articleDtoCids = request.ArticleContent.Select(c => c.LanguageId).ToList();
                    var removedContents = articleContentIds.Except(articleDtoCids);
                    _article.ArticleContents.Where(a => removedContents.Contains(a.LanguageId)).ToList().ForEach(removed =>
                    {
                        _ArticleRepository.Delete<ArticleContents>(removed);
                        _article.ArticleContents.Remove(removed);
                        contentToDelete.Add((int)removed.LanguageId);
                    });

                    // Update Related Countries 
                    foreach (var country in request.RelatedCountries)
                    {
                        var articleCountry =
                            _article.ArticleRelatedCountries
                                .FirstOrDefault(c => c.CountryId == country);
                        if (articleCountry != null) continue;
                        var newCountry = _ArticleRepository.getCountryById(country);
                        _article.ArticleRelatedCountries.Add(new ArticleRelatedCountries { Country = newCountry, CountryId = newCountry.CountryId });
                    }
                    var articleCountries = _article.ArticleRelatedCountries.Select(c => c.CountryId).ToList();
                    var dtoCountries = request.RelatedCountries;
                    var removedCountries = articleCountries.Except(dtoCountries);
                    _article.ArticleRelatedCountries.Where(a => removedCountries.Contains(a.CountryId))
                        .ToList()
                        .ForEach(removed => { _article.ArticleRelatedCountries.Remove(removed); });

                    // Update Related Country Groups 
                    foreach (var countryGroup in request.RelatedCountryGroups)
                    {
                        var articleCountryGroup =
                            _article.ArticleRelatedCountryGroups
                                .FirstOrDefault(c => c.CountryGroupId == countryGroup);
                        if (articleCountryGroup != null) continue;
                        var newCountryGroup = _ArticleRepository.getCountryGroupById(countryGroup);
                        _article.ArticleRelatedCountryGroups.Add(new ArticleRelatedCountryGroups { CountryGroup = newCountryGroup, CountryGroupId = newCountryGroup.CountryGroupId });
                    }
                    var articleCountryGroups = _article.ArticleRelatedCountryGroups.Select(c => c.CountryGroupId).ToList();
                    var dtoCountryGroups = request.RelatedCountryGroups;
                    var removedCountryGroups = articleCountryGroups.Except(dtoCountryGroups);
                    _article.ArticleRelatedCountryGroups.Where(a => removedCountryGroups.Contains(a.CountryGroupId))
                        .ToList()
                        .ForEach(removed => { _article.ArticleRelatedCountryGroups.Remove(removed); });

                    //update related articles
                    if (request.RelatedArticles != null)
                    {
                        var relatedArticles = _article.RelatedArticlesArticle.Select(c => c.ArticleId).ToList();
                        var dtoRelatedArticles = request.RelatedArticles;

                        var addRelatedArticles = dtoRelatedArticles.Except(relatedArticles);
                        var newRelatedArticles = _ArticleRepository.getArticlesListById(addRelatedArticles.ToList());// context.Articles.Where(a => addRelatedArticles.Contains(a.ArticleId));
                        newRelatedArticles.ForEach(a =>
                        {
                            _article.RelatedArticlesArticle.Add(new RelatedArticles { Article = a, RelatedArticleId = a.ArticleId });
                            //Reverse relation
                            //if (!a.RelatedArticlesArticle.Any(r => r.ArticleId == _article.ArticleId))
                            //{
                            //    a.RelatedArticlesArticle.Add(new RelatedArticles { Article = _article, RelatedArticleId = _article.ArticleId });
                            //}
                        });

                        var removedRelatedArticles = relatedArticles.Except(dtoRelatedArticles);
                        _article.RelatedArticlesArticle.Where(a => removedRelatedArticles.Contains(a.ArticleId))
                            .ToList()
                            .ForEach(removed =>
                            {
                                _article.RelatedArticlesArticle.Remove(removed);
                            });
                    }

                    // update related resources
                    if (request.RelatedResources != null)
                    {
                        foreach (var rResource in request.RelatedResources)
                        {
                            var relatedRes =
                                _article.RelatedResourcesArticle.FirstOrDefault(c => c.RelatedArticleId == rResource);
                            if (relatedRes != null) continue;
                            var newRelatedResource = _ArticleRepository.getArticleDataById(rResource);
                            _article.RelatedResourcesArticle.Add(new RelatedResources { Article = newRelatedResource, RelatedArticleId = newRelatedResource.ArticleId });
                        }
                        var relatedResources = _article.RelatedResourcesArticle.Select(c => c.RelatedArticleId).ToList();
                        var dtoRelatedResources = request.RelatedResources;
                        var removedRelatedResources = relatedResources.Except(dtoRelatedResources);
                        _article.RelatedResourcesArticle.Where(a => removedRelatedResources.Contains(a.RelatedArticleId))
                            .ToList()
                            .ForEach(removed => { _article.RelatedResourcesArticle.Remove(removed); });
                    }

                    //update related tags
                    if (request.RelatedTaxTags != null)
                    {
                        foreach (var rTag in request.RelatedTaxTags)
                        {
                            var relatedTag =
                                _article.ArticleRelatedTaxTags.FirstOrDefault(c => c.TaxTagId == rTag);
                            if (relatedTag == null)
                            {
                                var newTag = _ArticleRepository.getTaxTagsById(rTag);
                                _article.ArticleRelatedTaxTags.Add(new ArticleRelatedTaxTags { TaxTag = newTag, TaxTagId = newTag.TaxTagId });
                            }
                        }
                        var relatedTags = _article.ArticleRelatedTaxTags.Select(c => c.TaxTagId).ToList();
                        var dtoTags = request.RelatedTaxTags;
                        var removedTags = relatedTags.Except(dtoTags);
                        _article.ArticleRelatedTaxTags.Where(a => removedTags.Contains(a.TaxTagId))
                            .ToList()
                            .ForEach(removed => { _article.ArticleRelatedTaxTags.Remove(removed); });
                    }

                    //update related contacts
                    if (request.RelatedContacts != null)
                    {
                        foreach (var dtoContact in request.RelatedContacts)
                        {
                            var relatedContact = _article.ArticleRelatedContacts.FirstOrDefault(c => c.ContactId == dtoContact);
                            if (relatedContact == null)
                            {
                                var newContact = _ArticleRepository.getContactsById(dtoContact);// context.Contacts.FirstOrDefault(c => c.ContactId == dtoContact.ContactId);
                                _article.ArticleRelatedContacts.Add(new ArticleRelatedContacts { Contact = newContact, ContactId = newContact.ContactId });
                            }
                        }
                        var relatedContacts = _article.ArticleRelatedContacts.Select(c => c.ContactId).ToList();
                        var dtoContacts = request.RelatedContacts;
                        var removeContacts = relatedContacts.Except(dtoContacts);
                        _article.ArticleRelatedContacts.Where(a => removeContacts.Contains(a.ContactId))
                            .ToList()
                            .ForEach(removeContact => _article.ArticleRelatedContacts.Remove(removeContact));
                    }

                    //update image
                    _article.ImageId = request.ImageId;
                    //Push logic needs to be implemented
                    int userCount = _ArticleRepository.SendNotificationsForArticle<UpdateArticleCommand>(request);
                    if (userCount > 0) { _article.NotificationSentDate = DateTime.Now; }
                    await _ArticleRepository.UnitOfWork
                       .SaveEntitiesAsync();

                    response.IsSuccessful = true;
                    scope.Complete();
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
                    foreach (var content in _article.ArticleContents)
                    {
                        var doc = articleDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ArticleId") == _article.ArticleId
                                   && d.GetPropertyValue<int?>("LanguageId") == content.LanguageId);
                        var eventSourcing = new ArticleCommandEvent()
                        {                            
                            id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                            EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
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
                            RelatedArticles = _article.RelatedArticlesArticle.Select(s => new RelatedEntityId { IdVal = s.RelatedArticleId }).ToList(),
                            RelatedResources = _article.RelatedResourcesArticle.Select(s => new RelatedEntityId { IdVal = s.RelatedArticleId }).ToList(),
                            Discriminator = Constants.ArticlesDiscriminator,
                            PartitionKey = ""
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                    }
                    foreach (int i in contentToDelete)
                    {
                        var deleteEvt = new ArticleCommandEvent()
                        {
                            id = articleDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ArticleId") == _article.ArticleId
                                  && d.GetPropertyValue<int>("LanguageId") == i).GetPropertyValue<Guid>("id"),
                            EventType = ServiceBusEventType.Delete,
                            Discriminator = Constants.ArticlesDiscriminator,
                            PartitionKey = i.ToString()
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(deleteEvt);
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
