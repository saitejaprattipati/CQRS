using Author.Core.Framework.Utilities;
using Author.Query.API.GraphQL;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Author.Query.API.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly GraphQLQuery _graphQLQuery;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly IUtilityService _utilityService;
        private readonly IServiceProvider _serviceProvider;

        public GraphQLController(GraphQLQuery graphQLQuery, ISchema schema, IDocumentExecuter documentExecuter, IUtilityService utilityService, IServiceProvider serviceProvider)
        {
            _graphQLQuery = graphQLQuery;
            _schema = schema;
            _documentExecuter = documentExecuter;
            _utilityService = utilityService;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLParameter query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs,
                ////UserContext = Request.Headers,
                ExposeExceptions = _utilityService.IsTrusted(),
                //UserContext = new GraphQLUserContext { Headers = Request.Headers }
                ThrowOnUnhandledException = true,
            };

            //executionOptions.Listeners.Add(_serviceProvider.GetRequiredService<DataLoaderDocumentListener>());

            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);
            //throw new Exception("Exception while fetching all the students from the storage.");

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}