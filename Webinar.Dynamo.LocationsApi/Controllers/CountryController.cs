using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Webinar.Dynamo.Domain.Domain;
using Webinar.Dynamo.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryDomainService CountryDomainService;

        public CountryController(ICountryDomainService countryDomainService)
        {
            CountryDomainService = countryDomainService;
        }

        // GET: api/<CountryController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = CountryDomainService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<CountryController>
        [HttpPost]
        public IActionResult Post([FromBody] Country country)
        {
            try
            {
                bool result = CountryDomainService.Add(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
