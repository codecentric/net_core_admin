using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreAdmin;
using NetCoreAdmin.Beans;
using NetCoreAdmin.Health;
using System.Threading.Tasks;

namespace Nactuator
{
    [ApiController]
    [Route("[controller]")]
    public class ActuatorController : ControllerBase
    {
        private readonly ILogger<ActuatorController> logger;
        private readonly IEnvironmentProvider environmentProvider;
        private readonly IHealthProvider health;
        private readonly IBeanProvider beanProvider;

        public ActuatorController(ILogger<ActuatorController> logger, IEnvironmentProvider environmentProvider, IHealthProvider health, IBeanProvider beanProvider)
        {
            this.logger = logger;
            this.environmentProvider = environmentProvider;
            this.health = health;
            this.beanProvider = beanProvider;
        }

        [HttpOptions("env")]
        public ActionResult OptionsEnvironment()
        {
            
            return Ok();
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

        // todo environment single property

        [HttpGet("health")]
        public async Task<ActionResult<HealthData>> GetHealthAsync()
        {
            var data = await health.GetHealthAsync().ConfigureAwait(false);
            return Ok(data);
        }

        [HttpOptions("beans")]
        public ActionResult OptionsBeans()
        {

            return Ok();
        }

        [HttpGet("beans")]
        public ActionResult<BeanData> GetBeans()
        {
            var data = beanProvider.GetBeanData();
            var result = new JsonResult(data)
            {
                ContentType = "application/vnd.spring-boot.actuator.v3+json"
            };
            return result;
        }

    }
}
