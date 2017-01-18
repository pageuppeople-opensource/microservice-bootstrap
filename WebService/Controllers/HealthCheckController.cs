using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public class HealthCheckController : Controller
    {
        public StatusCodeResult Get()
        {
            return Ok();
        }
    }
    
}