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
                List<Articles> articlesDetails = new List<Articles>();
                List<TaxTags> taxTagsDetails = new List<TaxTags>();
                ResourceGroups ResourceGroupsDetails = new ResourceGroups();
                Provinces ProvincesDetails = new Provinces();
                Disclaimers DisclaimersDetails = new Disclaimers();
                List<Articles> rarticles = _ArticleRepository.getArticlesListById(request.RelatedArticles);
                List<Articles> rresources = _ArticleRepository.getArticlesListById(request.RelatedResources);
                List<TaxTags> rtaxTags = _ArticleRepository.getTaxTagsDetailsByIds(request.RelatedTaxTags);
                List<Countries> rcountries = _ArticleRepository.getCountriesByIds(request.RelatedCountries);
                List<CountryGroups> rcountryGroups = _ArticleRepository.getCountryGroupsByIds(request.RelatedCountryGroups);
                List<Contacts> rcontacts = _ArticleRepository.getContactsByIds(request.RelatedContacts);
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
                      //  var rArticleList = request.RelatedArticles;
                      //  var relatedArticles = rarticles.Where(a => rArticleList.Any(ra => ra == a.ArticleId)).ToList();
                        foreach (Articles article in rarticles)
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
                       // var rResourceList = request.RelatedResources.ToList();                       
                     //   var relatedResources = articles.Where(a => rResourceList.Contains(a.ArticleId)).ToList();
                        foreach (Articles article in rresources)
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
                      //  var rTagList = request.RelatedTaxTags.ToList();                        
                        //var relatedTags = taxTags.Where(t => rTagList.Contains(t.TaxTagId)).ToList();
                        foreach (TaxTags tags in rtaxTags)
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
                       // var rCountries = request.RelatedCountries.ToList();
                       // var relatedCountries = countries.Where(c => rCountries.Contains(c.CountryId)).ToList();
                        foreach (Countries country in rcountries)
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
                      //  var rCountryGroups = request.RelatedCountryGroups.ToList();
                       // var relatedCountryGroups = countryGroups.Where(c => rCountryGroups.Contains(c.CountryGroupId)).ToList();
                        foreach (CountryGroups cGroup in rcountryGroups)
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
                        foreach (Contacts rcontact in rcontacts)
                        {
                            _article.ArticleRelatedContacts.Add(new ArticleRelatedContacts()
                            {
                                ContactId = rcontact.ContactId,
                                Contact = rcontact
                            });
                        };
                    }
                    //Push logic needs to be implemented
                    int userCount = _ArticleRepository.SendNotificationsForArticle<CreateArticleCommand>(request);
                    if (userCount > 0) { _article.NotificationSentDate = DateTime.Now; }
                    _ArticleRepository.Add(_article);
                    await _ArticleRepository.UnitOfWork
                       .SaveEntitiesAsync();


                    articlesDetails = _ArticleRepository.getArticleCompleteDataById(_article.RelatedArticlesArticle.Select(s => s.RelatedArticleId).ToList());
                    taxTagsDetails = _ArticleRepository.getTaxTagsDetailsByIds(_article.ArticleRelatedTaxTags.Select(s => s.TaxTagId).ToList());
                    ResourceGroupsDetails = _article.ResourceGroupId == null ? null : _ArticleRepository.getResourceGroupById(int.Parse(_article.ResourceGroupId.ToString()));
                    ProvincesDetails = _article.ProvinceId == null ? null : _ArticleRepository.getProvisionsById(int.Parse(_article.ProvinceId.ToString()));
                    DisclaimersDetails = _article.DisclaimerId == null ? null : _ArticleRepository.getDisclaimerById(int.Parse(_article.DisclaimerId.ToString()));
                    response.IsSuccessful = true;
                    scope.Complete();
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var content in _article.ArticleContents)
                    {
                        var DisclaimerLanguageId = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;
                        var ResourceGroupLanguageId = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;
                        var ProvisionsLanguageId = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37;                     
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
                              Disclaimer = new DisclamersSchema { DisclaimerId=int.Parse(_article.DisclaimerId.ToString()), ProviderName= DisclaimersDetails.DisclaimerContents.Where(d=>d.LanguageId== DisclaimerLanguageId).Select(ds=>ds.ProviderName).FirstOrDefault(), ProviderTerms = DisclaimersDetails.DisclaimerContents.Where(d => d.LanguageId == DisclaimerLanguageId).Select(ds => ds.ProviderTerms).FirstOrDefault() },
                             ResourceGroup = new ResourceGroupsSchema {ResourceGroupId  = int.Parse(_article.ResourceGroupId.ToString()), GroupName = ResourceGroupsDetails.ResourceGroupContents.Where(d => d.LanguageId == ResourceGroupLanguageId).Select(ds => ds.GroupName).FirstOrDefault(), Position = ResourceGroupsDetails.Position },
                            IsPublished = _article.IsPublished,
                            CreatedDate = _article.CreatedDate,
                            CreatedBy = _article.CreatedBy,
                            UpdatedDate = _article.UpdatedDate,
                            UpdatedBy = _article.UpdatedBy,
                            NotificationSentDate = _article.NotificationSentDate,
                            Provisions = new ProvisionsSchema { ProvinceId = int.Parse(_article.ProvinceId.ToString()), DisplayName = ProvincesDetails.ProvinceContents.Where(d => d.LanguageId == ProvisionsLanguageId).Select(ds => ds.DisplayName).FirstOrDefault() },
                            ArticleContentId = content.ArticleContentId,
                            LanguageId = content.LanguageId,
                            Title = content.Title,
                            TeaserText = content.TeaserText,
                            Content = content.Content,
                            RelatedContacts = _article.ArticleRelatedContacts.Select(s => new RelatedEntityId { IdVal = s.ContactId }).ToList(),
                            RelatedCountries = _article.ArticleRelatedCountries.Select(s => new RelatedEntityId { IdVal = s.CountryId }).ToList(),
                            RelatedCountryGroups = _article.ArticleRelatedCountryGroups.Select(s => new RelatedEntityId { IdVal = s.CountryGroupId }).ToList(),
                            RelatedTaxTags = _article.ArticleRelatedTaxTags.Select(s => { var RelatedtaxTagLanguageId = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedTaxTagsSchema { TaxTagId = s.TaxTagId, DisplayName = taxTagsDetails.Where(td => td.TaxTagId == s.TaxTagId).FirstOrDefault().TaxTagContents.Where(ttc => ttc.LanguageId == RelatedtaxTagLanguageId).Select(ttcs => ttcs.DisplayName).FirstOrDefault() }; }).ToList(),
                            RelatedArticles = _article.RelatedArticlesArticle.Select(s => { var RelatedArticleLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedArticleLanguageId).Select(v => v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                            RelatedResources = _article.RelatedResourcesArticle.Select(s => { var RelatedResourceLanguageId = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == content.LanguageId).Count() > 0 ? content.LanguageId : 37; return new RelatedArticlesSchema { ArticleId = s.RelatedArticleId, PublishedDate = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).Select(v => v.PublishedDate).FirstOrDefault().ToString(), Title = articlesDetails.Where(ra => ra.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleContents.Where(ttc => ttc.LanguageId == RelatedResourceLanguageId).Select(v => v.Title).FirstOrDefault().ToString(), CountryId = articlesDetails.Where(ad => ad.ArticleId.Equals(s.RelatedArticleId)).FirstOrDefault().ArticleRelatedCountries.Select(arc => new RelatedEntityId { IdVal = arc.CountryId }).ToList() }; }).ToList(),
                            Discriminator = Constants.ArticlesDiscriminator,
                            PartitionKey = ""
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
