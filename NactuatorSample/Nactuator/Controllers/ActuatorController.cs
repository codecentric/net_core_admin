using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreAdmin;
using System.Threading.Tasks;

namespace Nactuator
{
    [ApiController]
    [Route("[controller]")]
    public class ActuatorController
    {
        private readonly ILogger<ActuatorController> logger;
        private readonly IEnvironmentProvider environmentProvider;
        private readonly IHealth health;

        public ActuatorController(ILogger<ActuatorController> logger, IEnvironmentProvider environmentProvider, IHealth health)
        {
            this.logger = logger;
            this.environmentProvider = environmentProvider;
            this.health = health;
        }

        [HttpOptions("env")]
        public ActionResult OptionsEnvironment()
        {
            
            return new OkResult();
        }

        [HttpGet("env")]
        public ActionResult<EnvironmentData> GetEnvironment()
        {
            var data = environmentProvider.GetEnvironmentData();
            var result = new JsonResult(data)
            {
                ContentType = "application/vnd.spring-boot.actuator.v3+json"
            };
            return result;
        }

        [HttpGet("health")]
        public async Task<ActionResult<EnvironmentData>> GetHealthAsync()
        {
            if (await health.GetHealthAsync().ConfigureAwait(false))
            {
                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        // todo environment single property

    }
}
