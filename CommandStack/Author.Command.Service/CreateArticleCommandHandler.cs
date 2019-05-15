using MediatR;
//using Microsoft.Extensions.Logging;
using Author.Command.Domain.Command;
using System;
using System.Threading.Tasks;
using Author.Command.Domain.Models;
using System.Collections.Generic;
using Author.Command.Persistence;
using System.Threading;

using System.Data.SqlClient;

using System.Data;
using Author.Command.Events;

namespace Author.Command.Service
{

    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CreateArticleCommandResponse>
    {
        private readonly ArticleRepository _context;
        private readonly IntegrationEventPublisherService _Eventcontext;
        //  private readonly ILogger _logger;

        public CreateArticleCommandHandler(IntegrationEventPublisherService Eventcontext)
        {
            _context = new ArticleRepository();
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
                var Status = await _context.CreateArticleDetails();

                //if (Status.UpdatedStatus.Contains("true"))
                //{
                var eventSourcing = new CreateArticleCommandEvent()
                {
                    Id = request.Id,
                    ArticleName = request.ArticleName,
                    ArticleCountry = request.ArticleCountry
                };
                await _Eventcontext.PublishThroughEventBusAsync(eventSourcing);
                response.IsSuccessful = true;
                //}
                //else
                //{
                //    //Log
                //}
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
