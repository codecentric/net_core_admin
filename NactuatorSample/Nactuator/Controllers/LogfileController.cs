using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Logfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
        public IActionResult Get([FromHeader(Name = "range")] RangeHeaderValue range)
        {
            // todo accept-ranges header
            // todo range header
            // todo what happens when header null, out of range etc? I think the doc stats that it throws then - write IT test

            // TODO return 206 when range set
            RangeItemHeaderValue rangeInfo;
            if (range != null && range.Ranges.Count > 0)
            {
                rangeInfo = range.Ranges.First();
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
            }
            else
            {
                rangeInfo = null!;
            }

            try
            {
                var data = logfileProvider.GetLog(rangeInfo?.From, rangeInfo?.To);

                return File(data, "text/plain");
            }
            catch (FileNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
