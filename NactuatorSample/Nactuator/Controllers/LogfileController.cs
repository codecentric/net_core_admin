using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Logfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetCoreAdmin.Controllers
{
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
        public async Task<ActionResult<string>> GetAsync([FromHeader(Name = "range")] RangeHeaderValue range)
        {
            // todo accept-ranges header
            // todo range header
            // todo what happens when header null, out of range etc? I think the doc stats that it throws then - write IT test
            RangeItemHeaderValue rangeInfo;
            if (range != null)
            {
                rangeInfo = range.Ranges.First();
            }
            else
            {
                rangeInfo = null!;
            }

            var data = await logfileProvider.GetLogAsync(rangeInfo?.From, rangeInfo?.To).ConfigureAwait(false);
            return Ok(data);
        }
    }
}
