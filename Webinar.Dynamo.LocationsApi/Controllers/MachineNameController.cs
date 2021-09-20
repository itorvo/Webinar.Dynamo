using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineNameController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetName()
        {
            try
            {
                var result = $"Hello from {Environment.MachineName}";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("namewithtime")]
        public IActionResult GetNameForTime()
        {
            try
            {
                Thread.Sleep(15000);
                var result = $"Hello from {Environment.MachineName}";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
