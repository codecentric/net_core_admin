using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NactuatorSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Demo code")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
            })
            .ToArray();
        }

        [HttpGet("stressgc")]
        [Produces("application/json")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Demo code")]
        public IEnumerable<WeatherForecast> StressGc()
        {
            // creates lots of objects for the gc to cleanup
            var rng = new Random();
            return Enumerable.Range(1, 50000).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
            })

            // yes, but this is intentionally inefficient
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified
            .ToArray().Take(50);
#pragma warning restore S2971 // "IEnumerable" LINQs should be simplified
        }

        [HttpPost]
        [Consumes("application/json")]
        public ActionResult PostTest(WeatherForecast forecast)
        {
            logger.LogInformation($"Got forecase: {forecast}");
            return Ok();
        }
    }
}
