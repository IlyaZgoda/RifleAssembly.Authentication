﻿using Microsoft.AspNetCore.Mvc;

namespace RifleAssembly.Authentication.Web.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Service is running");
        }
    }
}
