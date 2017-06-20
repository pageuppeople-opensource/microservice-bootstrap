using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("/healthcheck")]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        public StatusCodeResult Get()
        {
            return Ok();
        }
    }
    
}