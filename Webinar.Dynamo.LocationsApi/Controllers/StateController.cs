using Microsoft.AspNetCore.Mvc;
using System;
using Webinar.Dynamo.Domain.Domain;
using Webinar.Dynamo.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateDomainService StateDomainService;

        public StateController(IStateDomainService stateDomainService)
        {
            StateDomainService = stateDomainService;
        }

        // GET: api/<StateController>
        [HttpGet]
        public IActionResult Get(string paginationToken, int limit)
        {
            try
            {
                var result = StateDomainService.GetAll(paginationToken, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/<StateController>
        [HttpPatch]
        public IActionResult Patch([FromBody] State state)
        {
            try
            {
                var result = StateDomainService.Update(state);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<StateController>
        [HttpPost]
        public IActionResult Post([FromBody] State state)
        {
            try
            {
                var result = StateDomainService.Add(state);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<StateController>/5
        [HttpDelete]
        public IActionResult Delete(string country, string code)
        {
            try
            {
                var result = StateDomainService.Remove(country, code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
