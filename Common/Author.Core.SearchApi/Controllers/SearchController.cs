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
        /// <summary>
        /// This is Http POST action that creates index for search service
        /// </summary>
        /// <param name="indexName">This is indexName. Represents name of index to be created</param>
        /// <returns cref="IActionResult">
        ///     It Returns HTTP action results <see cref="OkObjectResult"/> with <see cref="Author.Core.SearchApi"/>.
        /// </returns>     
        /// <response code="200">Index Created</response>
        /// <response code="500">Internal server error message</response> 
        [HttpPost]
        [Route("createindex")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateIndex(string indexName)
        {
            _searchRepo.CreateIndex(indexName);
            await Task.CompletedTask;
            return Ok("Index Created");
        }
        /// <summary>
        /// This is Http POST action that deletes index for search service
        /// </summary>
        /// <param name="indexName">This is indexName. Represents name of index to be created</param>
        /// <returns cref="IActionResult">
        ///     It Returns HTTP action results <see cref="OkObjectResult"/> with <see cref="Author.Core.SearchApi"/>.
        /// </returns>     
        /// <response code="200">Index Deleted</response>
        /// <response code="500">Internal server error message</response> 
        [HttpPost]
        [Route("deleteindex")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> DeleteIndex(string indexName)
        {
            _searchRepo.DeleteIndex(indexName);
            await Task.CompletedTask;
            return Ok("Index Deleted");
        }
        /// <summary>
        /// This is Http POST action that inserts data into index for search service
        /// </summary>
        /// <param name="data">This is data. Represents data of the index</param>
        /// <param name="indexName">This is the indexName the data to be inserted</param>
        /// <returns cref="IActionResult">
        ///     It Returns HTTP action results <see cref="OkObjectResult"/> with <see cref="Author.Core.SearchApi"/>.
        /// </returns>     
        /// <response code="200">Data inserted into Index</response>
        /// <response code="500">Internal server error message</response> 
        [HttpPost]
        [Route("uploaddata")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UploadIndexData(List<publicindex> data, string searchIndex)
        {
            _searchRepo.UploadIndexData(data, searchIndex);
            await Task.CompletedTask;
            return Ok("Index Data Uploaded");
        }
    }
}