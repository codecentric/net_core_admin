using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NetCoreAdmin.Mappings;

namespace NetCoreAdmin.Controllers
{
    [ApiController]
    [Route("/actuator/mappings")]
    [Produces("application/vnd.spring-boot.actuator.v2", "application/json")]
    public class MappingController : ControllerBase
    {
        private readonly IMappingProvider mappingProvider;

        public MappingController(IMappingProvider mappingProvider)
        {
            this.mappingProvider = mappingProvider;
        }


        [HttpOptions()]
        public ActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {

            var result = new JsonResult(mappingProvider.GetCurrentMapping())
            {
                ContentType = Constants.ActuatorContentType
            };

            return result;
        }
    }
}
