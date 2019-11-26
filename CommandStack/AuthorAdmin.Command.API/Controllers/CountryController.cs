using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Author.Command.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AuthorAdmin.Command.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly IMediator _mediator;
        private ILogger<CountryController> _log;
        public CountryController(IMediator mediator, ILogger<CountryController> log)
        {
            _log = log;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("Image")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public void Image(/*HttpContextAccessor obj, */string FileName)
        {
            var file = Request.Form.Files[0];


            using (var memoryStream = new MemoryStream())
            {
                file.OpenReadStream().CopyTo(memoryStream);
                string bytes = Convert.ToBase64String(memoryStream.ToArray());
            }
            //string base64 = Convert.ToBase64String(bytes);
        } 

        /// <summary>
        /// Create Country
        /// </summary>
        /// <remarks>This API will create country</remarks>
        /// <param name="command">Create Country command object</param>
        /// <returns></returns>
       // [Authorize]
        [HttpPost]
        [Route("createCountry")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateCountry([FromForm] CreateCountryCommand command)
        {
            command = JsonConvert.DeserializeObject<CreateCountryCommand>(Request.Form.ToList()[0].Value.ToString());
            foreach (var file in Request.Form.Files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.OpenReadStream().CopyTo(memoryStream);
                    if (file.FileName.Contains("svg")) { command.ImagesData.SVGName = file.FileName; command.ImagesData.SVGData = Convert.ToBase64String(memoryStream.ToArray()); }
                    else { command.ImagesData.JPGName = file.FileName; command.ImagesData.JPGData = Convert.ToBase64String(memoryStream.ToArray()); }
                }
            }
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Update Country
        /// </summary>
        /// <remarks>This API will update Country</remarks>
        /// <param name="command">Update Country command object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateCountry")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> updateCountry([FromForm] UpdateCountryCommand command)
        {
            command = JsonConvert.DeserializeObject<UpdateCountryCommand>(Request.Form.ToList()[0].Value.ToString());
            foreach (var file in Request.Form.Files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.OpenReadStream().CopyTo(memoryStream);
                    if (file.FileName.Contains("svg")) { command.ImagesData.SVGName = file.FileName; command.ImagesData.SVGData = Convert.ToBase64String(memoryStream.ToArray()); }
                    else { command.ImagesData.JPGName = file.FileName; command.ImagesData.JPGData = Convert.ToBase64String(memoryStream.ToArray()); }
                }
            }
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }
        /// <summary>
        /// Manipulate Country
        /// </summary>
        /// <remarks>This API will manipulate country</remarks>
        /// <param name="command">Manipulate Country command object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("manipulateCountry")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ManipulateCountry([FromBody] ManipulateCountriesCommand command)
        {
            var response = await _mediator.Send(command);
            return CreateResponse(response);
        }

    }
}