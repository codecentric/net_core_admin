using Microsoft.AspNetCore.Mvc;
using NetCoreAdmin.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdmin.Controllers
{

    [ApiController]
    [Route("/actuator/beans")]
    public class BeanController : ControllerBase
    {
        private readonly IBeanProvider beanProvider;

        public BeanController(IBeanProvider beanProvider)
        {
            this.beanProvider = beanProvider;
        }

        [HttpOptions()]
        public ActionResult Options()
        {
            return Ok();
        }

        [HttpGet()]
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
