using System;
using System.Collections.Generic;
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
        public ActionResult<MetricsData> GetByName([FromRoute]string metric, [FromQuery] string? tag = "")
        {
            if (provider == null)
            {
                return StatusCode(403);
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                return GetByNameAndTag(metric, tag);
            }
            else
            {
                return GetName(metric);
            }
        }

        private ActionResult<MetricsData> GetName(string metric)
        {
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

        private ActionResult<MetricsData> GetByNameAndTag(string metric, string drilldown )
        {
            var splitted = drilldown.Split(':');
            if (splitted.Length != 2)
            {
                // ugly hack for the single sba request for: metrics/jvm.memory.used?tag=area:nonheap,id:Metaspace
                if (drilldown.ToUpperInvariant().Contains("METASPACE", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new MetricsData()
                    {
                        Name = "jvm.memory.used",
                        BaseUnit = "bytes",
                        Description = "The amount of used memory",
                        Measurements = new List<Measurement>()
                {
                    new Measurement
                    {
                        Statistic = "VALUE",
                        Value = 0,
                    },
                },
                    };
                    }

                return BadRequest(); // todo return rfc error
            }

            var tag = splitted[0];
            var value = splitted[1];

            var actuatorTag = new ActuatorTag(tag, value);
            try
            {
                var metricData = provider.GetMetricByNameAndTag(metric, actuatorTag);

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
