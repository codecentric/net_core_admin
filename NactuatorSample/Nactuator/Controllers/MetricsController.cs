using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreAdmin.Metrics;

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

        [HttpOptions]
        public ActionResult Options()
        {
            if (provider == null)
            {
                return StatusCode(403);
            }

            return Ok();
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            if (provider == null)
            {
                return StatusCode(403);
            }

            var result = new JsonResult(new { names = provider.GetMetricNames() })
            {
                ContentType = Constants.ActuatorContentType,
            };
            return result;
        }

        [HttpGet("{metric}")]
        public ActionResult<MetricsData> GetByName([FromRoute]string metric)
        {
            if (provider == null)
            {
                return StatusCode(403);
            }

            try
            {
                var metricData = provider.GetMetricByName(metric);

                var result = new JsonResult(metricData)
                {
                    ContentType = Constants.ActuatorContentType,
                };

                return result;
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
