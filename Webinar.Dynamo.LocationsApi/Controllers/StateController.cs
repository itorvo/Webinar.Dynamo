using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {

        public StateController()
        {
        }

        // GET: api/<StateController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = GetData();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private List<dynamic> GetData()
        {
            var txt = System.IO.File.ReadAllText(@"Resources/inputStates.txt");

            return System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(txt);
        }
    }
}
