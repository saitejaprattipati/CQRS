using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Author.Query.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICountryService _countryService;
        public CountryController(ILogger<CountryController> logger,
           ICountryService countryService)
        {
            _logger = logger;
            _countryService = countryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CountryResult), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Get([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 100 : pageSize;
            try
            {
                var countries = await _countryService.GetAllCountriesAsync("en-us");
                await Task.CompletedTask;
                return Ok(countries);
            }
            catch (Exception ex)
            {
                var error = $"Unable to retrieve countries details. {ex.Message}";
                _logger.LogError(ex, error);
                return StatusCode(500, error);
            }
        }
    }
}