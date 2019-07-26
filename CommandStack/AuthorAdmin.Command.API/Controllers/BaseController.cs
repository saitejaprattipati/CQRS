using Microsoft.AspNetCore.Mvc;
using System;


namespace AuthorAdmin.Command.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class BaseController : ControllerBase
    {
        public IActionResult CreateResponse<T>(T response)
        {
            if (response == null)
            {
                return BadRequest();
            }
            return Convert.ToBoolean(typeof(T).GetProperty("IsSuccessful").GetValue(response)) ? Ok() : (IActionResult)BadRequest();
        }
    }
}