﻿using Microsoft.AspNetCore.Mvc;
using Nactuator;
using NetCoreAdmin.Health;
using System.Threading.Tasks;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/health")]
    public class HealthController: ControllerBase
    {
        private readonly IHealthProvider health;

        public HealthController(IHealthProvider health)
        {
            this.health = health;
        }

        [HttpOptions()]
        public ActionResult Options()
        { 
            return Ok();
        }

        [HttpGet()]
        public async Task<ActionResult<HealthData>> GetAsync()
        {
            var data = await health.GetHealthAsync().ConfigureAwait(false);
            return Ok(data);
        }

    }
}
