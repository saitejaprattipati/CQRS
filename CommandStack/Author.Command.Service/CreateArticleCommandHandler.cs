//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using Author.Command.Events;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                Articles Article = new Articles();
                Article.State = "Test1";
                Article.PublishedDate=DateTime.Now;
                Article.UpdatedDate= DateTime.Now;
                Article.CreatedDate = DateTime.Now;
                _ArticleRepository.Add(Article);
                //  var Status = await _context.CreateArticleDetails();
                var eventSourcing = new CreateArticleCommandEvent()
                {
                    EventType = "ArticleCreation",
                    Id = request.Id,
                    ArticleName = request.ArticleName,
                    ArticleCountry = request.ArticleCountry
                };
                //   using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                //  {
                await _ArticleRepository.UnitOfWork
                   .SaveEntitiesAsync();
               //     throw new Exception(string.Format("Unable to find a client by id"));
                    await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                    response.IsSuccessful = true;

                //    scope.Complete();
             //   }
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
