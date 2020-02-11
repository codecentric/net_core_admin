using Microsoft.AspNetCore.Mvc;
using Nactuator;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/env")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentProvider environmentProvider;

        public EnvironmentController(IEnvironmentProvider environmentProvider)
        {
            this.environmentProvider = environmentProvider;
        }

        [HttpOptions]
        public ActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        public ActionResult<EnvironmentData> Get()
        {
            var data = environmentProvider.GetEnvironmentData();
            var result = new JsonResult(data)
            {
                ContentType = Constants.ActuatorContentType,
            };
            return result;
        }
    }
}
