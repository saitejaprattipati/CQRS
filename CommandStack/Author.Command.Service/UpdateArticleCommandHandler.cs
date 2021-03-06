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
                Disclaimers DisclaimersDetails = new Disclaimers();
                ResourceGroups ResourceGroupsDetails = new ResourceGroups();
                Provinces ProvincesDetails = new Provinces();
                List<TaxTags> taxTagsDetails = new List<TaxTags>();
                List<Articles> articlesDetails = new List<Articles>();
                Articles _article = _ArticleRepository.getArticleCompleteDataById(new List<int> { request.ArticleID })[0];
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
                    taxTagsDetails = _ArticleRepository.getTaxTagsDetailsByIds(_article.ArticleRelatedTaxTags.Select(s => s.TaxTagId).ToList());
                    articlesDetails = _ArticleRepository.getArticleCompleteDataById(_article.RelatedArticlesArticle.Select(s => s.RelatedArticleId).ToList());
                    ResourceGroupsDetails = _article.ResourceGroupId == null ? null : _ArticleRepository.getResourceGroupById(int.Parse(_article.ResourceGroupId.ToString()));
                    ProvincesDetails = _article.ProvinceId == null ? null : _ArticleRepository.getProvisionsById(int.Parse(_article.ProvinceId.ToString()));
                    DisclaimersDetails = _article.DisclaimerId == null ? null : _ArticleRepository.getDisclaimerById(int.Parse(_article.DisclaimerId.ToString()));
                    response.IsSuccessful = true;
                    scope.Complete();
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var articleDocs = _context.GetAll(Constants.ArticlesDiscriminator);
                    foreach (var content in _article.ArticleContents)
                    {
                        foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == content.LanguageId))
                        {
                            foreach (var relatedArticles in article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles"))
                            {
                                if (relatedArticles.ArticleId == _article.ArticleId)
                                {
                                    List<RelatedArticlesSchema> relatedArticleSchema = new List<RelatedArticlesSchema>();
                                    relatedArticleSchema = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles");

                                    var index = relatedArticleSchema.IndexOf(relatedArticleSchema.Where(i => i.ArticleId == _article.ArticleId).First());
                                    if (index != -1)
                                        relatedArticleSchema[index] = new RelatedArticlesSchema { ArticleId = _article.ArticleId, Title = content.Title == null ? "" : content.Title, PublishedDate = _article.PublishedDate == null ? "" : _article.PublishedDate.ToString(), CountryId = _article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList() };                               
                                    var eventSourcingRelated = new ArticleCommandEvent()
                                    {
                                        id = article != null ? article.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                                        EventType = ServiceBusEventType.Update,
                                        ArticleId = article.GetPropertyValue<int>("ArticleId"),
                                        PublishedDate =article.GetPropertyValue<string>("PublishedDate"),
                                        Author =  article.GetPropertyValue<string>("author"),
                                        ImageId =  article.GetPropertyValue<int>("ImageId"),
                                        State =article.GetPropertyValue<string>("State"),
                                        Type =   article.GetPropertyValue<int>("Type"),
                                        SubType =article.GetPropertyValue<int>("SubType"),
                                        ResourcePosition = article.GetPropertyValue<int>("ResourcePosition"),
                                        Disclaimer =article.GetPropertyValue<DisclamersSchema>("Disclaimer"),
                                        ResourceGroup = article.GetPropertyValue<ResourceGroupsSchema>("ResourceGroup"),
                                        IsPublished = article.GetPropertyValue<bool>("IsPublished"),
                                        CreatedDate =  article.GetPropertyValue<string>("CreatedDate"),
                                        CreatedBy =article.GetPropertyValue<string>("CreatedBy"),
                                        UpdatedDate = article.GetPropertyValue<string>("UpdatedDate"),
                                        UpdatedBy = article.GetPropertyValue<string>("UpdatedBy"),
                                        NotificationSentDate = article.GetPropertyValue<string>("NotificationSentDate"),
                                        Provinces = article.GetPropertyValue<ProvinceSchema>("Provisions"),
                                        ArticleContentId = article.GetPropertyValue<int>("ArticleContentId"),
                                        LanguageId =  article.GetPropertyValue<int>("LanguageId"),
                                        Title =  article.GetPropertyValue<string>("Title"),
                                        TitleInEnglishDefault = article.GetPropertyValue<string>("TitleInEnglishDefault"),
                                        TeaserText = article.GetPropertyValue<string>("TeaserText"),
                                        Content = article.GetPropertyValue<string>("Content"),
                                        RelatedContacts =  article.GetPropertyValue<List<RelatedEntityId>>("RelatedContacts"),
                                        RelatedCountries = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountries"),
                                        RelatedCountryGroups = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountryGroups"),
                                        RelatedTaxTags = article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags"),
                                        RelatedArticles = relatedArticleSchema,
                                        RelatedResources = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedResources"),
                                        Discriminator = article.GetPropertyValue<string>("Discriminator"),
                                        PartitionKey = ""
                                    };
                                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                                }
                            }
                        }
                        var DisclaimerLanguageId = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;
                        var ResourceGroupLanguageId = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;
                        var ProvisionsLanguageId = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;
                        var doc = articleDocs.FirstOrDefault(d => d.GetPropertyValue<int>("ArticleId") == _article.ArticleId
                                   && d.GetPropertyValue<int?>("LanguageId") == content.LanguageId);
                        var eventSourcing = new ArticleCommandEvent()
                        {
                            id = doc != null ? doc.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                            EventType = doc != null ? ServiceBusEventType.Update : ServiceBusEventType.Create,
                            ArticleId = _article.ArticleId,
                            PublishedDate = _article.PublishedDate == null ? "" : _article.PublishedDate.ToString(),
                            Author = _article.Author == null ? "" : _article.Author,
                            ImageId = _article.ImageId == null ? -1 : _article.ImageId,
                            State = _article.State == null ? "" : _article.State,
                            Type = _article.Type == null ? -1 : _article.Type,
                            SubType = _article.SubType == null ? -1 : _article.SubType,
                            ResourcePosition = _article.ResourcePosition == null ? -1 : _article.ResourcePosition,
                            Disclaimer = new DisclamersSchema { DisclaimerId = int.Parse(_article.DisclaimerId==null?"-1": _article.DisclaimerId.ToString()), ProviderName = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == DisclaimerLanguageId).Select(ds => ds.ProviderName == null ? "" : ds.ProviderName).FirstOrDefault(), ProviderTerms = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == DisclaimerLanguageId).Select(ds => ds.ProviderTerms == null ? "" : ds.ProviderTerms).FirstOrDefault() },
                            ResourceGroup = new ResourceGroupsSchema { ResourceGroupId = int.Parse(_article.ResourceGroupId == null ? "-1" : _article.ResourceGroupId.ToString()), GroupName = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == ResourceGroupLanguageId).Select(ds => ds.GroupName == null ? "" : ds.GroupName).FirstOrDefault(), Position = ResourceGroupsDetails.Position == null ? -1 : ResourceGroupsDetails.Position },
                            IsPublished = _article.IsPublished == null ? false : _article.IsPublished,
                            CreatedDate = _article.CreatedDate == null ? "" : _article.CreatedDate.ToString(),
                            CreatedBy = _article.CreatedBy == null ? "" : _article.CreatedBy,
                            UpdatedDate = _article.UpdatedDate == null ? "" : _article.UpdatedDate.ToString(),
                            UpdatedBy = _article.UpdatedBy == null ? "" : _article.UpdatedBy,
                            NotificationSentDate = _article.NotificationSentDate == null ? "" : _article.NotificationSentDate.ToString(),
                            Provinces = new ProvinceSchema { ProvinceId = int.Parse(_article.ProvinceId.ToString()), DisplayName = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == ProvisionsLanguageId).Select(ds => ds.DisplayName == null ? "" : ds.DisplayName).FirstOrDefault() },
                            ArticleContentId = content.ArticleContentId == null ? -1 : content.ArticleContentId,
                            LanguageId = content.LanguageId == null ? -1 : content.LanguageId,
                            Title = content.Title == null ? "" : content.Title,
                            TitleInEnglishDefault = _article.ArticleContents.Where(l => l.LanguageId == 37 && l.ArticleId == content.ArticleId).Select(s => s.Title == null ? "" : s.Title).FirstOrDefault(),
                            TeaserText = content.TeaserText == null ? "" : content.TeaserText,
                            Content = content.Content == null ? "" : content.Content,
                            RelatedContacts = _article.ArticleRelatedContacts.Select(s => new RelatedEntityId { IdVal = s.ContactId }).ToList(),
                            RelatedCountries = _article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList(),
                            RelatedCountryGroups = _article.ArticleRelatedCountryGroups.Select(s => new RelatedEntityId { IdVal = s.CountryGroupId }).ToList(),
                            RelatedTaxTags = _article.ArticleRelatedTaxTags.Select(s => { var RelatedtaxTagLanguageId = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedTaxTagsSchema { TaxTagId = s.TaxTagId, DisplayName = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == RelatedtaxTagLanguageId).Select(ttcs => ttcs.DisplayName == null ? "" : ttcs.DisplayName).FirstOrDefault() }; }).ToList(),
                            RelatedArticles = _article.RelatedArticlesArticle.Select(s => { var RelatedArticleLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate == null ? "" : v.PublishedDate.ToString()).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedArticleLanguageId).Select(v => v.Title == null ? "" : v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                            RelatedResources = _article.RelatedResourcesArticle.Select(s => { var RelatedResourceLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate == null ? "" : v.PublishedDate.ToString()).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedResourceLanguageId).Select(v => v.Title == null ? "" : v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                            Discriminator = Constants.ArticlesDiscriminator,
                            PartitionKey = ""
                        };
                        await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                    }
                    foreach (int i in contentToDelete)
                    {
                        foreach (var article in articleDocs.Where(ad => ad.GetPropertyValue<int>("LanguageId") == i))
                        {
                            foreach (var relatedArticles in article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles"))
                            {
                                if (relatedArticles.ArticleId == _article.ArticleId)
                                {
                                    var titleInEnglish=articleDocs.Where(ad => ad.GetPropertyValue<int>("ArticleId") == _article.ArticleId && ad.GetPropertyValue<int>("LanguageId") == 37).Select(ads => ads.GetPropertyValue<string>("Title")).FirstOrDefault();
                                    List<RelatedArticlesSchema> relatedArticleSchema = new List<RelatedArticlesSchema>();
                                    relatedArticleSchema = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedArticles");

                                    var index = relatedArticleSchema.IndexOf(relatedArticleSchema.Where(ras => ras.ArticleId == _article.ArticleId).First());
                                    if (index != -1)
                                        if (titleInEnglish == "") relatedArticleSchema.Remove(relatedArticleSchema.Where(ras => ras.ArticleId == _article.ArticleId).First()); else relatedArticleSchema[index] = new RelatedArticlesSchema { ArticleId = _article.ArticleId, Title = (titleInEnglish == null ? "" : titleInEnglish), PublishedDate = _article.PublishedDate == null ? "" : _article.PublishedDate.ToString(), CountryId = _article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList() };
                                    var eventSourcingRelated = new ArticleCommandEvent()
                                    {
                                        id = article != null ? article.GetPropertyValue<Guid>("id") : Guid.NewGuid(),
                                        EventType = ServiceBusEventType.Update,
                                        ArticleId = article.GetPropertyValue<int>("ArticleId"),
                                        PublishedDate = article.GetPropertyValue<string>("PublishedDate"),
                                        Author = article.GetPropertyValue<string>("author"),
                                        ImageId = article.GetPropertyValue<int>("ImageId"),
                                        State = article.GetPropertyValue<string>("State"),
                                        Type = article.GetPropertyValue<int>("Type"),
                                        SubType = article.GetPropertyValue<int>("SubType"),
                                        ResourcePosition = article.GetPropertyValue<int>("ResourcePosition"),
                                        Disclaimer = article.GetPropertyValue<DisclamersSchema>("Disclaimer"),
                                        ResourceGroup = article.GetPropertyValue<ResourceGroupsSchema>("ResourceGroup"),
                                        IsPublished = article.GetPropertyValue<bool>("IsPublished"),
                                        CreatedDate = article.GetPropertyValue<string>("CreatedDate"),
                                        CreatedBy = article.GetPropertyValue<string>("CreatedBy"),
                                        UpdatedDate = article.GetPropertyValue<string>("UpdatedDate"),
                                        UpdatedBy = article.GetPropertyValue<string>("UpdatedBy"),
                                        NotificationSentDate = article.GetPropertyValue<string>("NotificationSentDate"),
                                        Provinces = article.GetPropertyValue<ProvinceSchema>("Provisions"),
                                        ArticleContentId = article.GetPropertyValue<int>("ArticleContentId"),
                                        LanguageId = article.GetPropertyValue<int>("LanguageId"),
                                        Title = article.GetPropertyValue<string>("Title"),
                                        TitleInEnglishDefault = article.GetPropertyValue<string>("TitleInEnglishDefault"),
                                        TeaserText = article.GetPropertyValue<string>("TeaserText"),
                                        Content = article.GetPropertyValue<string>("Content"),
                                        RelatedContacts = article.GetPropertyValue<List<RelatedEntityId>>("RelatedContacts"),
                                        RelatedCountries = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountries"),
                                        RelatedCountryGroups = article.GetPropertyValue<List<RelatedEntityId>>("RelatedCountryGroups"),
                                        RelatedTaxTags = article.GetPropertyValue<List<RelatedTaxTagsSchema>>("RelatedTaxTags"),
                                        RelatedArticles = relatedArticleSchema,
                                        RelatedResources = article.GetPropertyValue<List<RelatedArticlesSchema>>("RelatedResources"),
                                        Discriminator = article.GetPropertyValue<string>("Discriminator"),
                                        PartitionKey = ""
                                    };
                                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcingRelated);
                                }
                            }
                        }
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
