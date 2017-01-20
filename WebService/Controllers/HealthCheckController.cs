using System;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("/")]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        public StatusCodeResult Get()
        {
            // Return OK if current minute is even
            // otherwise return NotFound.
            if (DateTime.UtcNow.Minute % 2 == 0)
                return Ok();
            return NotFound();
        }
    }
    
}