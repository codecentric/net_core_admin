using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreAdmin.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/metrics")]
    [Produces("application/vnd.spring-boot.actuator.v2", "application/json")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> logger;
        private readonly IMetricsProvider provider;

        public MetricsController(ILogger<MetricsController> logger, IMetricsProvider provider = null!)
        {
            this.logger = logger;
            this.provider = provider;

            if (provider == null)
            {
                this.logger.LogInformation("No IMetricsProvider installed, no metrics available. Consider using the Package NetCoreAdmin.Metrics");
            }
        }

        [HttpOptions()]
        public ActionResult Options()
        {
            if (provider == null)
            {
                return StatusCode(403);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetAsync()
        {
            if (provider == null)
            {
                return StatusCode(403);
            }

            return Ok(provider.GetMetricNamesAsync());
        }

        [HttpGet("{metric}")]
        public async Task<ActionResult<MetricsData>> GetByNameAsync([FromRoute]string metric)
        {

            if (provider == null)
            {
                return StatusCode(403);
            }

            try
            {
                var result = await provider.GetMetricByNameAsync(metric);

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                logger.LogDebug("'{metric}' not found", metric);
                return NotFound();
            }
        }

        // todo drilldown
    }
}
