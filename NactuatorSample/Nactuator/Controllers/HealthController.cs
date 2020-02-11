using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Health;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/health")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthProvider health;

        public HealthController(IHealthProvider health)
        {
            this.health = health;
        }

        [HttpOptions]
        public ActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<HealthData>> GetAsync()
        {
            var data = await health.GetHealthAsync().ConfigureAwait(false);
            return new JsonResult(data)
            {
                ContentType = Constants.ActuatorContentType,
            };
        }
    }
}