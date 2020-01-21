using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Core.SearchApi.Domain;
using Author.Core.SearchApi.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Author.Core.SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private ILogger<SearchController> _log;
        private ISearchRepository _searchRepo;
        public SearchController(ILogger<SearchController> log, ISearchRepository searchRepo)
        {
            _log = log;
            _searchRepo = searchRepo;
        }
        [HttpPost]
        [Route("createindex")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task CreateIndex(string indexName)
        {
            _searchRepo.CreateIndex(indexName);
            await Task.CompletedTask;
        }
        [HttpPost]
        [Route("deleteindex")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task DeleteIndex(string indexName)
        {
            _searchRepo.DeleteIndex(indexName);
            await Task.CompletedTask;
        }
        [HttpPost]
        [Route("uploaddata")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task UploadIndexData(List<publicindex> data)
        {
            _searchRepo.UploadIndexData(data);
            await Task.CompletedTask;
        }
    }
}