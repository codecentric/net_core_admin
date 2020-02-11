using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Threaddump;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/threaddump")]
    [Produces("application/json")]
    public class ThreadDumpController : ControllerBase
    {
        private readonly IThreadDumpProvider threadDumpProvider;

        public ThreadDumpController(IThreadDumpProvider threadDumpProvider)
        {
            this.threadDumpProvider = threadDumpProvider;
        }

        [HttpOptions]
        public ActionResult Options()
        {
            if (!threadDumpProvider.IsEnabled)
            {
                return StatusCode(403);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (!threadDumpProvider.IsEnabled)
            {
                return StatusCode(403);
            }

            var result = new JsonResult(threadDumpProvider.GetThreadDump())
            {
                ContentType = Constants.ActuatorContentType,
            };

            return result;
        }
    }
}
