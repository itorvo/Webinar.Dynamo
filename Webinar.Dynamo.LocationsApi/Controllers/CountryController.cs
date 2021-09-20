using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {

        public CountryController()
        {
        }

        // GET: api/<CountryController>
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
            var txt = System.IO.File.ReadAllText(@"Resources/inputCountries.txt");
            return System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(txt);
        }
    }
}
