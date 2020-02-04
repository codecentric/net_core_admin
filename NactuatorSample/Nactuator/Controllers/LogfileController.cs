using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Logfile;
using System.IO;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/logfile")]
    public class LogfileController: ControllerBase
    {
        private readonly ILogfileProvider logfileProvider;

        public LogfileController(ILogfileProvider logfileProvider)
        {
            this.logfileProvider = logfileProvider;
        }

        [HttpOptions()]
        public ActionResult Options()
        {
            return Ok();
        }

        [HttpGet()]
        public IActionResult Get()
        {
            try
            {
                var data = logfileProvider.GetLog();

                return File(data, "text/plain", enableRangeProcessing: true);
            }
            catch (FileNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
