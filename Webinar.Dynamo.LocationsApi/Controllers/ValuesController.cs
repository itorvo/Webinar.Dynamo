using Microsoft.AspNetCore.Mvc;

namespace Webinar.Dynamo.LocationsApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                id = $"{System.DateTime.UtcNow.AddHours(-5).Ticks}",
                message = "hola mundo"
            });
        }
    }
}
