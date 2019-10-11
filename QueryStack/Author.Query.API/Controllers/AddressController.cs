using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Author.Query.Persistence;

namespace Author.Query.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAddressRepository _addressRepo;

        public AddressController(ILogger<AddressController> logger,
           IAddressRepository addressRepo)
        {
            _logger = logger;
            _addressRepo = addressRepo;
        }
        /// <summary>
        /// This is Http GET action that returns pagged list of addresses
        /// </summary>
        /// <param name="pageNumber">This is Page Number. Represents current Page</param>
        /// <param name="pageSize">This is page Size. Represents number of records per page</param>
        /// <returns cref="IActionResult">
        ///     It Returns HTTP action results <see cref="OkObjectResult"/> with <see cref="Author.Query.Domain.AddressAggregateDetails"/>.
        /// </returns>     
        /// <response code="200">Paged list of Address collections</response>
        /// <response code="500">Internal server error message</response> 
        [HttpGet]
        [ProducesResponseType(typeof(Author.Query.Domain.AddressAggregateDetails), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Get([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 100 : pageSize;
            try
            {
                var Addresses = await _addressRepo.Get(pageNumber, pageSize);
                await Task.CompletedTask;
                return Ok(Addresses);
            }
            catch (Exception ex)
            {
                var error = $"Unable to retrieve address details. {ex.Message}";
                _logger.LogError(ex, error);
                return StatusCode(500, error);
            }
        }

        /// <summary>
        /// This is HTTP Get By ID action that returns address object for the passed address ID.
        /// If not found returns Not found error.
        /// </summary>
        /// <param name="addressId">The Address object's ID</param>
        /// <returns cref="IActionResult">
        /// If Found, returns the Address object with HTTP 200 status if found else returns 404 result
        /// </returns>
        /// <response code="200">Requested address object</response>  
        /// <response code="404">Object not found error message</response>  
        /// <response code="500">Internal server error message</response> 
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Author.Query.Domain.AddressAggregateDetails), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetById(string id,string partitionKey)
        {
            try
            {
                var address =  await _addressRepo.GetById(id, partitionKey);
                if (address != null)
                    return Ok(address);
                else
                    return NotFound($"No address found by id {id}");
            }
            catch (Exception ex)
            {
                var error = $"Unable to retrieve address for id {id}. {ex.Message}";
                _logger.LogError(ex, error);
                return StatusCode(500, error);
            }
        }
    }
}