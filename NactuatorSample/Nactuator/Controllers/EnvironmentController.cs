using System.Collections.Generic;
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

        [HttpGet("{prop}")]
        public ActionResult<SinglePropertyResult> GetSingle(string prop)
        {
            var data = environmentProvider.GetEnvironmentData();

            foreach (var source in data.PropertySources)
            {
                if (source.Properties.ContainsKey(prop))
                {
                    var value = source.Properties[prop];
                    var result = new SinglePropertyResult(new PropertyResult() { Source = source.Name, Value = value.Value },  new List<string>(), new List<PropertySources> { source });
                    var jsonResult = new JsonResult(result)
                    {
                        ContentType = Constants.ActuatorContentType,
                    };
                    return jsonResult;
                 }
            }

            return NotFound();
        }
    }
}
