using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Nactuator
{
    [ApiController]
    [Route("[controller]")]
    public class ActuatorController
    {
        private readonly ILogger<ActuatorController> logger;
        private readonly IEnvironmentProvider environmentProvider;

        public ActuatorController(ILogger<ActuatorController> logger, IEnvironmentProvider environmentProvider)
        {
            this.logger = logger;
            this.environmentProvider = environmentProvider;
        }

        [HttpGet("environment")]
        public async Task<ActionResult<EnvironmentData>> GetEnvironmentAsync()
        {
            var data = environmentProvider.GetEnvironmentData();
            var result = new JsonResult(data)
            {
                ContentType = "application/vnd.spring-boot.actuator.v3+json"
            };
            return result;
        }

        // todo single property

    }
}
